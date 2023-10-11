using CarRentalApp.Core.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalApp.Infrastructure.Data
{
    public class CarRentalDbContext : IdentityDbContext<ApplicationUser>
    {
        public CarRentalDbContext(DbContextOptions<CarRentalDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Rent>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            var adminRoleId = "5ad93cd7-35e6-4fc7-9690-714f86ec8ef2";
            var userRoleId = "ac5e271a-005b-4ec8-8bdd-86571bdcdb1a";
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId,
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Rent> Rents { get; set; }
    }
}
