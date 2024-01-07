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

        private static IServiceCollection AddOutBoxInterceptedDbContext<TContext>(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> options) where TContext : DbContext
        {
            services.AddScoped<ConvertDomainEventsToOutboxMessagesInterceptor>();

            services.AddDbContext<TContext>((sp, optionBuilder) =>
            {
                var interceptor = sp
                    .GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();
                optionBuilder.AddInterceptors(interceptor!);
                options(sp, optionBuilder);
            });

            services.AddDbContext<OutBoxDbContext>((sp, optionBuilder) =>
            {
                options(sp, optionBuilder);
            });

            return services;
        }

        private static IServiceCollection AddOutBoxInterceptedDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where TContext : DbContext
        {
            services.AddScoped<ConvertDomainEventsToOutboxMessagesInterceptor>();

            services.AddDbContext<TContext>((sp, optionBuilder) =>
            {
                var interceptor = sp
                    .GetService<ConvertDomainEventsToOutboxMessagesInterceptor>();
                optionBuilder.AddInterceptors(interceptor!);
                options(optionBuilder);
            });

            services.AddDbContext<OutBoxDbContext>((sp, optionBuilder) =>
            {
                options(optionBuilder);
            });


            return services;
        }
    }
}
