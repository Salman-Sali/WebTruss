using Microsoft.EntityFrameworkCore;

namespace WebTruss.BackgroundJob
{
    public class OutBoxDbContext : DbContext, IOutBoxDbContext
    {
        public OutBoxDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<OutBoxMessage> OutBoxMessages { get; set; }
    }
}
