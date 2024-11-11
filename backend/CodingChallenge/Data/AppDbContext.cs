using CodingChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<CryptoPrice> CryptoPrices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CryptoPrice>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // Adjust the precision and scale as needed
        }
    }
}
