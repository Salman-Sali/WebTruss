using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using System.Security.Policy;
using System.Text;

namespace WebTruss.BackgroundJob
{
    [DisallowConcurrentExecution]
    public class ProcessOutBoxMessagesJob : IJob
    {
        private readonly IPublisher _publisher;
        private readonly IConfiguration configuration;
        private readonly OutBoxDbContext dbContext;

        public ProcessOutBoxMessagesJob(IPublisher publisher,
            IConfiguration configuration,
            OutBoxDbContext dbContext)
        {
            _publisher = publisher;
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var messages = await dbContext
                .OutBoxMessages
                .Where(a => a.ProcessedOnUtc == null)
                .Take(1)
                .ToListAsync();

            foreach (var message in messages)
            {
                var domainEvent = JsonConvert.DeserializeObject<BaseEvent>(
                    message.Content,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });

                if (domainEvent is null)
                    continue;

                try
                {
                    await _publisher.Publish(domainEvent, context.CancellationToken);
                }
                catch (Exception ex)
                {
                    message.Error = UnwrapException(ex);
                }
                message.ProcessedOnUtc = DateTime.Now;
            }

            await dbContext.SaveChangesAsync();
            if (messages.Count == 0)
                await Task.Delay(10 * 1000);
        }

        public static string UnwrapException(Exception exception)
        {
            var exceptions = GetExceptions(exception);
            var sb = new StringBuilder();
            foreach (var item in exceptions)
            {
                sb.AppendLine(item.Message);
                sb.AppendLine(item.StackTrace);
            }

            return sb.ToString().Trim();
        }

        private static List<Exception> GetExceptions(Exception exception, List<Exception> exceptions = null!)
        {
            exceptions ??= new List<Exception>();

            exceptions.Add(exception);
            if (exception.InnerException != null)
            {
                return GetExceptions(exception.InnerException, exceptions);
            }
            else
            {
                exceptions.Reverse();
                return exceptions;
            }
        }
    }
}
