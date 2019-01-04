using Data.Contracts.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementation
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserEntity>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        DbSet<ApplicationUserEntity> ApplicationUsers { get; set; }
        DbSet<RoleEntity> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            RoleEntity adminRole = new RoleEntity { Id = 1, Name = adminRoleName };
            RoleEntity userRole = new RoleEntity { Id = 2, Name = userRoleName };

            builder.Entity<RoleEntity>().HasData(new RoleEntity[] { adminRole, userRole });

            base.OnModelCreating(builder);
        }
    }
}
