using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using OAuthPractice.Common.Utils;
using OAuthPractice.Contracts;
using OAuthPractice.Entity;

namespace OAuthPractice.Services
{
    public class GoogleService : IExternalOAuthService
    {
        private readonly GoogleInformation _googleInformation;

        public GoogleService(IOptions<GoogleInformation> googleInformation)
        {
            _googleInformation = googleInformation.Value;
        }

        public Providers Providers => Providers.Google;

        public async Task<UserSocialInfoDto> TranslateToken(string token, CancellationToken ct)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _googleInformation.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            return new UserSocialInfoDto(payload.GivenName, payload.FamilyName, payload.Email, payload.Subject);
        }
    }
}
