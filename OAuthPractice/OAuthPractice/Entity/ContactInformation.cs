namespace OAuthPractice.Entity
{
    public class ContactInformation
    {
        public ContactInformation(string gmail, string? phoneNumber)
        {
            Gmail = gmail;
            PhoneNumber = phoneNumber;
        }

        private ContactInformation() //ef
        {

        }
        public string Gmail { get; private set; }
        public string? PhoneNumber { get; private set; }
    }
}

