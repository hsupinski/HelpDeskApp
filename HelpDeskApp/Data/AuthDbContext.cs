using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var userRoleId = "2392bbf7-ff10-4104-adcf-89c9d1edd91b";
            var consultantRoleId = "5e9fbf20-0979-434b-9f17-9d26452e876f";
            var departmentHeadId = "7f309e1d-d6e1-48a0-b117-d3e68ea26e91";
            var adminRoleId = "dfccad92-2414-41eb-9367-6003dd8bbc10";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                },

                new IdentityRole
                {
                    Id = consultantRoleId,
                    ConcurrencyStamp = consultantRoleId,
                    Name = "Consultant",
                    NormalizedName = "CONSULTANT"
                },

                new IdentityRole
                {
                    Id = departmentHeadId,
                    ConcurrencyStamp = departmentHeadId,
                    Name = "Department Head",
                    NormalizedName = "DEPARTMENT HEAD"
                },

                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
