using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Plate> Plates { get; set; }
        public DbSet<StatusChangeLog> StatusChangeLogs { get; set; }
    }
}
