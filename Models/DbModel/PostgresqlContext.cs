using Microsoft.EntityFrameworkCore;

namespace HOGI.Models.DbModel
{
    public class PostgresqlContext : DbContext
    {
        public PostgresqlContext(DbContextOptions<PostgresqlContext> options) : base(options){}

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(o => o.Id);
        }
    }
}