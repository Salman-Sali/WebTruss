using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebTruss.Authentication.JWT
{
    public class JWTService
    {
        private readonly IJWTConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public JWTService(IJWTConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Tokens GenerateTokens(ClaimsIdentity claimsIdentity)
        {
            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
            byte[] bytes = Encoding.UTF8.GetBytes(this.configuration.Key);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claimsIdentity,
                Expires = new DateTime?(DateTime.UtcNow.AddMinutes((double)this.configuration.AccessTokenLifetimeInMinutes)),
                SigningCredentials = new SigningCredentials((SecurityKey)new SymmetricSecurityKey(bytes), "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256")
            };
            SecurityToken token = securityTokenHandler.CreateToken(tokenDescriptor);
            string refreshToken = this.GenerateRefreshToken(claimsIdentity);
            return new Tokens
            {
                AccessToken = securityTokenHandler.WriteToken(token),
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken(ClaimsIdentity claimsIdentity)
        {
            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
            byte[] bytes = Encoding.UTF8.GetBytes(this.configuration.RefreshKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claimsIdentity,
                Expires = new DateTime?(DateTime.UtcNow.AddMinutes((double)this.configuration.RefreshTokenLifetimeInMinutes)),
                SigningCredentials = new SigningCredentials((SecurityKey)new SymmetricSecurityKey(bytes), "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256")
            };
            SecurityToken token = securityTokenHandler.CreateToken(tokenDescriptor);
            return securityTokenHandler.WriteToken(token);
        }

        public bool ValidateRefreshToken(string refreshToken) => this.ValidateToken(refreshToken, this.configuration.RefreshKey);

        public bool ValidateToken(string token) => this.ValidateToken(token, this.configuration.Key);

        public bool ValidateToken()
        {
            string token = this.GetToken();
            return !string.IsNullOrEmpty(token) && this.ValidateToken(token);
        }

        public string? GetToken()
        {
            this.httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var value);
            return value[0]?.Split(" ")[1];
        }

        private bool ValidateToken(string token, string secretKey)
        {
            string issuer = this.configuration.Issuer;
            string audience = this.configuration.Audience;
            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidAudience = audience,
                ValidateIssuer = false,
                ValidIssuer = issuer,
                ValidateLifetime = true,
                IssuerSigningKey = (SecurityKey)new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuerSigningKey = true
            };
            try
            {
                securityTokenHandler.ValidateToken(token, validationParameters, out SecurityToken _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string? GetClaimValue(string token, string claimKey)
        {
            return new JwtSecurityTokenHandler()
                .ReadJwtToken(token)
                .Claims
                .Where(x => x.Type == claimKey)
                .Select(x => x.Value)
                .FirstOrDefault();
        }

        public string? GetClaimValue(string claimKey)
        {
            return new JwtSecurityTokenHandler()
                .ReadJwtToken(this.GetToken())
                .Claims
                .Where(x => x.Type == claimKey)
                .Select(x => x.Value)
                .FirstOrDefault();
        }
    }
}
