namespace WebTruss.Notifications
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string Address { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SenderName { get; set; } = null!;
        public string Host { get; set; } = null!;
        public bool Ssl { get; set; }
        public int Port { get; set; }
    }
}