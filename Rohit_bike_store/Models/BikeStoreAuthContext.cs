using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rohit_bike_store.Models
{
    public class BikeStoreAuthContext : IdentityDbContext
    {
        public BikeStoreAuthContext(DbContextOptions<BikeStoreAuthContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var adminRoleId = "248bec2e-9ebe-4cbe-82d4-6aac85ebc62b";
            var storeRoleId = "c6140040-cd18-4d75-bc6c-25a4242503db";
            var staffRoleId = "50182c84-8fdb-4ee3-88af-ba7734621d8f";


            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin" ,
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id=storeRoleId,
                    ConcurrencyStamp=storeRoleId,
                    Name = "Store",
                    NormalizedName = "Store".ToUpper()
                },
               
                new IdentityRole
                {
                    Id=staffRoleId,
                    ConcurrencyStamp=staffRoleId,
                    Name = "Staff",
                    NormalizedName = "Staff".ToUpper()
                }

            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}

