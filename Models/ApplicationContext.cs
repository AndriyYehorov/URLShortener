using Microsoft.EntityFrameworkCore;
using URLShortener.Enums;

namespace URLShortener.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<URL> URLs { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Text> Texts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var roles = Enum
                .GetValues<RolesEnum>()
                .Select(r => new Role
                {
                    Id = (int)r,
                    Name = r.ToString()
                });

            modelBuilder.Entity<Role>().HasData(roles);   
        }

    }
}
