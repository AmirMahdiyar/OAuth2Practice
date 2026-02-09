using OAuthPractice.Contracts;
using OAuthPractice.Entity;

namespace OAuthPractice.Services
{
    public class OAuthStrategyRepository : IOAuthStrategyRepository
    {
        private readonly IEnumerable<IExternalOAuthService> _externalOAuthServices;

        public OAuthStrategyRepository(IEnumerable<IExternalOAuthService> externalOAuthServices)
        {
            _externalOAuthServices = externalOAuthServices;
        }

        public IExternalOAuthService GetStrategy(Providers providers)
        {
            return _externalOAuthServices
                   .Single(x => EqualityComparer<Providers>.Default.Equals(x.Providers, providers));
        }
    }
}
