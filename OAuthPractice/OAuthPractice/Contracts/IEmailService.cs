namespace OAuthPractice.Contracts
{
    public interface IEmailService
    {
        Task SendMail(EmailSetting setting, CancellationToken ct);
    }

    public class EmailSetting
    {
        public EmailSetting(string toEmail, string subject, string body)
        {
            ToEmail = toEmail;
            Subject = subject;
            Body = body;
        }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
