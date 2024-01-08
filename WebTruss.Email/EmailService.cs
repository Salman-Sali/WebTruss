using MimeKit;
using MailKit.Net.Smtp;

namespace WebTruss.Notifications
{
    public class EmailService(IEmailConfiguration emailConfiguration) : IEmailService
    {
        private readonly IEmailConfiguration emailConfiguration = emailConfiguration;

        public async Task SendEmailAsync(string email, string username, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailConfiguration.SenderName, emailConfiguration.Address));
            message.To.Add(new MailboxAddress(username, email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailConfiguration.Host, emailConfiguration.Port, emailConfiguration.Ssl);
                await client.AuthenticateAsync(emailConfiguration.Address, emailConfiguration.Password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }
}
