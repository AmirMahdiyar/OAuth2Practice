using OAuthPractice.Entity.Base;

namespace OAuthPractice.Entity
{
    public class User : BaseEntity<Guid>
    {
        private List<UserAuthentications> _authentications = new();
        private User() { } // for ef

        private User(string name, string lastName, string username, ContactInformation contactInformation)
        {
            Id = Guid.NewGuid();
            ContactInformation = contactInformation;
            Name = name;
            LastName = lastName;
            Username = username;
        }

        public static User Create(string name, string lastName, string username, ContactInformation contactInformation)
        {
            return new User(name, lastName, username, contactInformation);
        }
        public string Name { get; private set; }
        public string LastName { get; private set; }

        public string Username { get; private set; }

        public ContactInformation ContactInformation { get; private set; }
        public IEnumerable<UserAuthentications> Authentications => _authentications.AsEnumerable();


        #region Behaviors
        public void AddAuthentication(UserAuthentications authentication) => _authentications.Add(authentication);
        public void RemoveAuthentication(UserAuthentications authentication) => _authentications.Remove(authentication);

        #endregion

    }
}

