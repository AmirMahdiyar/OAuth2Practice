namespace OAuthPractice.Common.Utils
{
    public class JwsInformationOptions
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Expires { get; set; }
    }
}