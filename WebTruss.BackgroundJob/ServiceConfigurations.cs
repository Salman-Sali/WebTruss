using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace WebTruss.BackgroundJob
{
    public static class ServiceConfigurations
    {
        public static IServiceCollection AddOutBoxBackgroundJob(
            this IServiceCollection services,
            IOutBoxDbContext context)
        {
            services.AddSingleton(context);

            services.AddQuartz(configure =>
            {
                if (true)
                {
                    var jobKey = new JobKey(nameof(ProcessOutBoxMessagesJob));

                    configure
                        .AddJob<ProcessOutBoxMessagesJob>(jobKey)
                        .AddTrigger(trigger =>
                            trigger.ForJob(jobKey)
                                .WithSimpleSchedule(
                                    schedule =>
                                        schedule.WithIntervalInSeconds(1)
                                            .RepeatForever()));
                }
            });
            services.AddQuartzHostedService();

            return services;
        }

        public static IServiceCollection AddOutBoxInterceptor(this IServiceCollection services, DbContextOptionsBuilder options)
        {
            services.AddScoped<ConvertDomainEventsToOutboxMessagesInterceptor>();
            var interceptor = services
                .BuildServiceProvider()
                .GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();
            options.AddInterceptors(interceptor!);
            return services;
        }
    }
}
