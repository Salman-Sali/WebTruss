using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;

namespace WebTruss.Authentication
{
    public class JWTHandler
    {
        public static string? GenerateToken(List<Claim> claims, JWTConfigration jwtConfigration)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtConfigration.ExpiryInMinutes),
                Issuer = jwtConfigration.Issuer,
                Audience = jwtConfigration.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtConfigration.SecretKey)),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static bool ValidateToken(string token, JWTConfigration jwtConfigration, DateTime? tokenInvalidatedDateTime = null)
        {
            if (tokenInvalidatedDateTime != null)
            {
                if (GetTokenIssuedDateTime(token) < tokenInvalidatedDateTime)
                {
                    return false;
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = jwtConfigration.Audience,
                ValidateIssuer = true,
                ValidIssuer = jwtConfigration.Issuer,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtConfigration.SecretKey)),
                ValidateIssuerSigningKey = true
            };
            SecurityToken validatedToken;
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<Claim> GetClaims(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.ToList();
        }

        public static string GetClaimValue(string token, string claimKey)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.Where(x => x.Type == claimKey).Select(x => x.Value).FirstOrDefault();
        }

        public static DateTime GetTokenIssuedDateTime(string tokenStr)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenStr);
            return token.ValidFrom;
        }

        public static string GetJwtToken(IHttpContextAccessor httpContextAccessor)
        {
            HttpContext context = httpContextAccessor.HttpContext;
            if (context.Request.Headers.TryGetValue("Authorization", out var token))
            {
                return token[0].Split(" ")[1];
            }
            return null;
        }
    }
}
