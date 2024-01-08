namespace WebTruss.Notifications
{
    public interface IEmailConfiguration
    {
        public string Address { get; set; }
        public string Password { get; set; }
        public string SenderName { get; set; }
        public string Host { get; set; }
        public bool Ssl { get; set; }
        public int Port { get; set; }
    }
}
