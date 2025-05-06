using System.Security.Principal;
using Catalog.Domain.Entities;

namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Plate> Plates { get; set; }
        public DbSet<PlateDetail> PlateDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlateDetail>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasOne<Plate>()
                    .WithMany()
                    .HasForeignKey(m => m.PlateId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
