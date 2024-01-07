using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace WebTruss.BackgroundJob
{
    public sealed class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            DbContext? dbContext = eventData.Context;

            if (dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var domainEvents = dbContext
                .ChangeTracker
                .Entries<IHaveEvents>()
                .Select(x => x.Entity)
                .Select(x => x.DomainEvents)
                .ToList();

            var outBoxMessages = domainEvents
                .SelectMany(x => x)
                .Select(x => new OutBoxMessage
                {
                    Id = Guid.NewGuid(),
                    OccurredOnUtc = DateTime.Now,
                    Type = x.GetType().Name,
                    Content = JsonConvert.SerializeObject(
                        x,
                        new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All
                        })
                })
                .ToList();

            domainEvents.ForEach(x => x.Clear());

            dbContext.Set<OutBoxMessage>().AddRange(outBoxMessages);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
