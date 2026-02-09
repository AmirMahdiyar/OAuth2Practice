using Microsoft.AspNetCore.Mvc;
using OAuthPractice.Contracts;
using OAuthPractice.Entity;

namespace OAuthPractice.Controllers
{
    /// <summary>
    /// Manages user-related operations including registration, login, and external authentication.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IOAuthStrategyRepository _oAuthStrategyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwsService _jwsService;
        private readonly IEmailService _emailService;

        public UserController(IOAuthStrategyRepository oAuthStrategyRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IJwsService jwsService, IEmailService emailService)
        {
            _oAuthStrategyRepository = oAuthStrategyRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwsService = jwsService;
            _emailService = emailService;
        }

        /// <summary>
        /// Authenticates a user via external providers (OAuth2).
        /// </summary>
        /// <param name="dto">The OAuth data containing access token and the chosen provider.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <remarks>
        /// Validates the external token, registers the user if they don't exist, 
        /// or links the new provider to an existing account if not already linked.
        /// </remarks>
        /// <response code="200">Returns a custom JWS token upon successful authentication.</response>
        /// <response code="400">If the token is invalid or an error occurs during processing.</response>
        [HttpPost("Login/OAuth")]
        public async Task<IActionResult> LoginWithOAuth([FromBody] OAuthLoginDto dto, CancellationToken ct)
        {
            var chosenStrategy = _oAuthStrategyRepository.GetStrategy(dto.Providers);

            var payload = await chosenStrategy.TranslateToken(dto.accessToken, ct);
            var user = await _userRepository.GetUser(payload.gmail, ct);
            if (user is null)
            {
                return await AddAndLoginUser(chosenStrategy, payload, ct);
            }
            if (!user.Authentications.Any(x => x.Providers == chosenStrategy.Providers))
            {
                await AddOAuthToUser(payload, user, ct);
                Ok(await _jwsService.CreateJwsToken(user, ct));
            }

            return BadRequest("Something Went Wrong");

        }

        /// <summary>
        /// Registers a new user with a local account (Username/Password).
        /// </summary>
        /// <param name="dto">User registration details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">If the username is already taken or data is invalid.</response>
        [HttpPost("SignIn")]
        public async Task<IActionResult> Signin([FromBody] SignInDto dto, CancellationToken ct)
        {
            if (await IsUserExists(dto.username, ct))
                return BadRequest("User already exists");

            await CreateNewUser(dto, ct);

            return Ok("You are signedIn Successfully");
        }

        /// <summary>
        /// Authenticates a user using local credentials (Username and Password).
        /// </summary>
        /// <param name="dto">Login credentials.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <response code="200">Returns a JWS token if credentials are valid.</response>
        /// <response code="400">If the username or password is incorrect.</response>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] RegularLoginDto dto, CancellationToken ct)
        {
            var user = await _userRepository.GetUserByUsername(dto.username, ct);
            if (user is null)
                return BadRequest("User not found");

            if (user.Authentications.Any(x => x.ProviderKey == dto.password && x.Providers == Entity.Providers.Local))
                return Ok(await _jwsService.CreateJwsToken(user, ct));

            return BadRequest("Username Or Password is wrong  !");


        }

        /// <summary>
        /// Updates or sets a new password using a temporary JWT received via email.
        /// </summary>
        /// <param name="dto">The new password details.</param>
        /// <param name="jwt">The temporary security token from the email link.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <remarks>
        /// This endpoint is triggered when a user clicks the "Change Password" link in their email.
        /// It updates the existing local password or creates a new local authentication if none exists.
        /// </remarks>
        /// <response code="200">Password updated/set successfully.</response>
        /// <response code="400">If the token is invalid or the user is not found.</response>
        [HttpPost("ChangePassword/{jwt}")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, [FromRoute] string jwt, CancellationToken ct)
        {
            var principal = await _jwsService
                .TranslateToken(jwt, ct);

            var user = await _userRepository.GetUser
                (Guid.Parse(principal.Claims.Single(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value), ct);
            if (user is null) return BadRequest("User Not Found !");

            if (user.Authentications.Any(x => x.Providers == Entity.Providers.Local))
            {
                var localAuth = user.Authentications.SingleOrDefault(x => x.Providers == Entity.Providers.Local);

                localAuth!
                    .ChangeProviderKey(dto.newPassword);

                await _unitOfWork.Commit(ct);

                return Ok("Password Has Been Changed successfully");
            }
            else
            {
                var localAuth = new UserAuthentications(user.Id, Providers.Local, dto.newPassword);

                user.AddAuthentication(localAuth);
                await _unitOfWork.Commit(ct);

                return Ok("Password Has Been Set successfully");

            }
        }

        /// <summary>
        /// Sends a password reset/set link to the user's registered email.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the user.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <remarks>
        /// Typically called from a logged-in user's profile to initiate a password change or setup via email verification.
        /// </remarks>
        /// <response code="200">Email sent successfully.</response>
        /// <response code="400">If the user is not found.</response>
        [HttpGet("{id}/UserAuthentications/Password")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid id, CancellationToken ct)
        {
            var user = await _userRepository.GetUser(id, ct);
            if (user is null) return BadRequest("User Not Found");
            var userTempToken = await _jwsService.CreateJwsToken(user, ct);
            var emailSetting = new EmailSetting(user.ContactInformation.Gmail, "ChangePassword", $"https://localhost:7223/ChangePassword/{userTempToken}");

            await _emailService.SendMail(emailSetting, ct);
            return Ok("A Linked Is Sent in Your Mail check it out");
        }

        #region Private Methods
        private async Task<IActionResult> AddAndLoginUser(IExternalOAuthService chosenStrategy, UserSocialInfoDto payload, CancellationToken ct)
        {
            var newUser = OAuthPractice.Entity.User.Create(payload.firstName, payload.familyName, payload.gmail, new Entity.ContactInformation(payload.gmail, null));
            newUser.AddAuthentication(new Entity.UserAuthentications(newUser.Id, chosenStrategy.Providers, payload.Key));
            _userRepository.AddUser(newUser);
            await _unitOfWork.Commit(ct);
            return Ok(await _jwsService.CreateJwsToken(newUser, ct));
        }

        private async Task AddOAuthToUser(UserSocialInfoDto payload, User user, CancellationToken ct)
        {
            user.AddAuthentication(new Entity.UserAuthentications(user.Id, Entity.Providers.Google, payload.Key));
            await _unitOfWork.Commit(ct);

        }

        private async Task CreateNewUser(SignInDto dto, CancellationToken ct)
        {
            var newUser = Entity.User.Create(dto.name, dto.lastName, dto.username, new Entity.ContactInformation(dto.gmail, dto.phoneNumber));
            newUser.AddAuthentication(new Entity.UserAuthentications(newUser.Id, Entity.Providers.Local, dto.password));
            _userRepository.AddUser(newUser);
            await _unitOfWork.Commit(ct);
        }

        private async Task<bool> IsUserExists(string username, CancellationToken ct)
        {
            var oldUser = await _userRepository.GetUserByUsername(username, ct);
            return oldUser is not null;
        }


        #endregion

    }

    #region Dtos
    public record OAuthLoginDto(string accessToken, Providers Providers);
    public record RegularLoginDto(string username, string password);
    public record SignInDto(string name, string lastName, string username, string password, string gmail, string? phoneNumber);
    public record ChangePasswordDto(string newPassword);
    #endregion
}
