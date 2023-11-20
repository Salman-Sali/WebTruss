namespace WebTruss.Authentication.JWT
{
    public interface IJWTConfiguration
    {
        string Key { get; set; }

        string RefreshKey { get; set; }

        string Issuer { get; set; }

        string Audience { get; set; }

        int AccessTokenLifetimeInMinutes { get; set; }

        int RefreshTokenLifetimeInMinutes { get; set; }
    }

    public class JWTConfiguration : IJWTConfiguration
    {
        public string Key { get; set; } = null!;

        public string RefreshKey { get; set; } = null!;

        public string Issuer { get; set; } = null!;

        public string Audience { get; set; } = null!;

        public int AccessTokenLifetimeInMinutes { get; set; }

        public int RefreshTokenLifetimeInMinutes { get; set; }
    }
}