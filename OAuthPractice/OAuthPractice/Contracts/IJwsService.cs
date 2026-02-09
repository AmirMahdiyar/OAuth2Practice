using OAuthPractice.Entity;
using System.Security.Claims;

namespace OAuthPractice.Contracts
{
    public interface IJwsService
    {
        Task<string> CreateJwsToken(User user, CancellationToken cancellationToken);
        Task<ClaimsPrincipal> TranslateToken(string token, CancellationToken cancellationToken);
    }
}
