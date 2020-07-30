using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeployBot.Infrastructure.Database
{
    public class DeployBotDbContext : DbContext
    {
        public DeployBotDbContext()
        {
            
        }

        public DeployBotDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Release> Releases { get; set; }
        public DbSet<Deployment> Deployments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Release>();

            modelBuilder.Entity<Deployment>()
                .Property(x => x.Status)
                .HasConversion(new EnumToNumberConverter<DeploymentStatus, int>());
        }
    }
}