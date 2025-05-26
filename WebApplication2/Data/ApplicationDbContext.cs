// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Rundown> Rundowns { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(c => c.Currency_id);

                // Настройка связи с Rundown
                entity.HasMany(c => c.Rundowns)
                    .WithOne(r => r.Currency)
                    .HasForeignKey(r => r.Currency_id)
                    .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
            });

            modelBuilder.Entity<Rundown>(entity =>
            {
                entity.HasKey(r => r.Rundown_id);

                entity.Property(r => r.Rundown_value)
                    .HasColumnType("decimal(9, 4)");

                // Настройка связи с Currency
                entity.HasOne(r => r.Currency)
                    .WithMany(c => c.Rundowns)
                    .HasForeignKey(r => r.Currency_id)
                    .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
            });
        }

    }
}