using OAuthPractice.Entity.Base;

namespace OAuthPractice.Entity
{
    public class UserAuthentications : BaseEntity<Guid>
    {
        private UserAuthentications() //ef
        {

        }
        public UserAuthentications(Guid userId, Providers providers, string providerKey)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Providers = providers;
            ProviderKey = providerKey;
        }

        public Guid UserId { get; private set; }
        public Providers Providers { get; private set; }
        public string ProviderKey { get; private set; }


        #region Behaviors
        public void ChangeProviderKey(string newProviderKey) => ProviderKey = newProviderKey;

        #endregion

    }
}

