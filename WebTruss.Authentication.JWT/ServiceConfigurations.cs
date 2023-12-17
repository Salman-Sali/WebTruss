using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebTruss.Aws.Configuration;

namespace WebTruss.Authentication.JWT
{
    public static class ServiceConfigurations
    {
        public static IServiceCollection AddWebTrussJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            ServiceProvider provider = services.BuildServiceProvider();
            if (provider.GetService<IJWTConfiguration>() == null)
            {
                JWTConfiguration jwtConfiguration = new JWTConfiguration();
                AwsLambdaConfigurator.Bind(jwtConfiguration, configuration);
                services.AddSingleton<IJWTConfiguration>(jwtConfiguration);
            }
            services.AddAuthentication((x =>
            {
                x.DefaultAuthenticateScheme = "Bearer";
                x.DefaultChallengeScheme = "Bearer";
                x.DefaultScheme = "Bearer";
            })).AddJwtBearer();
            services.AddAuthorization();
            services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
            services.AddSingleton<JWTService>();
            if (provider.GetService<IHttpContextAccessor>() == null)
                services.AddHttpContextAccessor();
            return services;
        }
    }

    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IJWTConfiguration jwtConfig;

        public ConfigureJwtBearerOptions(IJWTConfiguration myService)
        {
            jwtConfig = myService;
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            if (name == JwtBearerDefaults.AuthenticationScheme)
            {
                var key = Encoding.UTF8.GetBytes(jwtConfig.Key);
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            }
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(string.Empty, options);
        }
    }
}
