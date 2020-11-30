using BillTracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillTracker.Repositories
{
    public class BillsContext : DbContext
    {
        public BillsContext(DbContextOptions<BillsContext> options)
            : base(options)
        {
        }

        public DbSet<Bill> Bills { get; set; }
    }
}
