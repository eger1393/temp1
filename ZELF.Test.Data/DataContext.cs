using Microsoft.EntityFrameworkCore;
using ZELF.Test.Data.Models;

namespace ZELF.Test.Data
{
    
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.SubscribersCount);
        }
    }
}