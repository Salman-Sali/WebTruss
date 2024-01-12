using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebTruss.Notifications
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddWebTrussEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailConfigurationService = services
                .BuildServiceProvider()
                .GetService<IEmailConfiguration>();

            if(emailConfigurationService != null)
            {
                EmailConfiguration emailConfiguration = new();
                configuration.Bind(emailConfigurationService);
                services.AddSingleton(emailConfiguration);
            }

            services.AddSingleton<IEmailService, EmailService>();

            return services;
        }
    }
}
