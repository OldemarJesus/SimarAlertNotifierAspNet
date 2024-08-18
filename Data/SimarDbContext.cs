using Microsoft.EntityFrameworkCore;
using SimarAlertNotifier.Models;

namespace SimarAlertNotifier.Data
{
    public class SimarDbContext : DbContext
    {
        public SimarDbContext(DbContextOptions<SimarDbContext> options) : base(options)
        {
        }

        public DbSet<Subscriber> Subscribers { get; set; }
    }
}
