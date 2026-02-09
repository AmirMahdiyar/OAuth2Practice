using OAuthPractice.Entity;

namespace OAuthPractice.Contracts
{
    public interface IOAuthStrategyRepository
    {
        IExternalOAuthService GetStrategy(Providers providers);
    }
}
