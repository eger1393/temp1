using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.SubscribersCount);
            modelBuilder.Entity<User>().Property(x => x.Name).HasMaxLength(64);
        }
    }
}