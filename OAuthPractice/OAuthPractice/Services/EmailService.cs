using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using OAuthPractice.Common.Utils;
using OAuthPractice.Contracts;

namespace OAuthPractice.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailInformation _emailInfo;

        public EmailService(IOptions<EmailInformation> emailInfo)
        {
            _emailInfo = emailInfo.Value;
        }

        public async Task SendMail(EmailSetting setting, CancellationToken ct)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailInfo.SenderEmail);
            email.To.Add(MailboxAddress.Parse(setting.ToEmail));
            email.Subject = setting.Subject;

            var builder = new BodyBuilder { HtmlBody = setting.Body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailInfo.SmtpServer,
                                    int.Parse(_emailInfo.SmtpPort),
                                    MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailInfo.SenderEmail, _emailInfo.Password, ct);
            await smtp.SendAsync(email, ct);
            await smtp.DisconnectAsync(true, ct);
        }
    }
}
