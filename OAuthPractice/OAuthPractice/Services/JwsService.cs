using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuthPractice.Common.Utils;
using OAuthPractice.Contracts;
using OAuthPractice.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace OAuthPractice.Services
{
    public class JwsService : IJwsService
    {
        private readonly JwsInformationOptions _jwsOptions;

        public JwsService(IOptions<JwsInformationOptions> jwsOptions)
        {
            _jwsOptions = jwsOptions.Value;
        }

        public async Task<string> CreateJwsToken(User user, CancellationToken cancellationToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwsOptions.Key));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Name , user.Name),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwsOptions.Issuer,
                audience: _jwsOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(int.Parse(_jwsOptions.Expires)),
                signingCredentials: signingCredentials
                );

            await Task.CompletedTask;
            var jwsToken = new JwtSecurityTokenHandler().WriteToken(token);


            return jwsToken;

        }

        public async Task<ClaimsPrincipal> TranslateToken(string token, CancellationToken cancellationToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwsOptions.Key));
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = _jwsOptions.Issuer,
                ValidAudience = _jwsOptions.Audience,
                IssuerSigningKey = key
            }, out _);

            await Task.CompletedTask;

            return principal;
        }
    }
}
