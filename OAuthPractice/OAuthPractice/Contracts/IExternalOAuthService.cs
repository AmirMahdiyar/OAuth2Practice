using OAuthPractice.Entity;

namespace OAuthPractice.Contracts
{
    public interface IExternalOAuthService
    {
        public Providers Providers { get; }
        Task<UserSocialInfoDto> TranslateToken(string token, CancellationToken ct);
    }

    public record UserSocialInfoDto(string firstName, string familyName, string gmail, string Key);
}
