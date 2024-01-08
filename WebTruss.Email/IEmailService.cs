namespace WebTruss.Notifications
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string email, string username, string subject, string body);
    }
}
