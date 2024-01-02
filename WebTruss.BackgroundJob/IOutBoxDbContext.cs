using Microsoft.EntityFrameworkCore;

namespace WebTruss.BackgroundJob
{
    public interface IOutBoxDbContext
    {
        public DbSet<OutBoxMessage> OutBoxMessages { get; set; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
