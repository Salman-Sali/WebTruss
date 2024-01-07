using Microsoft.EntityFrameworkCore;

namespace WebTruss.BackgroundJob
{
    public class OutBoxDbContext : DbContext, IOutBoxDbContext
    {
        public DbSet<OutBoxMessage> OutBoxMessages { get; set; }
    }
}
