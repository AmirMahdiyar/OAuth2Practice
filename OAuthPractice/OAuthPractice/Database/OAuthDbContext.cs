using Microsoft.EntityFrameworkCore;
using OAuthPractice.Database.Configuration;
using OAuthPractice.Entity;

namespace OAuthPractice.Database
{
    public class OAuthDbContext : DbContext
    {
        public OAuthDbContext(DbContextOptions<OAuthDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAuthentications> UserAuthentications { get; set; }

    }
}
