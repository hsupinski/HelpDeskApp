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

            var adminUserId = "ad17f1b2-9f32-48b9-a52c-b1f27a0adda8";
            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                ConcurrencyStamp = adminUserId,
                UserName = "superadmin",
                NormalizedUserName = "SUPERADMIN",
                Email = "admin@test.com",
                NormalizedEmail = "ADMIN@TEST.COM",
                EmailConfirmed = true,
                SecurityStamp = string.Empty
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "LtrWR;gF_,U5:6*");

            builder.Entity<IdentityUser>().HasData(adminUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
        }

        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure the database is created
                context.Database.EnsureCreated();

                // Check if roles exist, if not create them
                string[] roleNames = { "User", "Consultant", "Department Head", "Admin" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Check if superadmin exists, if not create it
                var superadmin = await userManager.FindByNameAsync("superadmin");
                if (superadmin == null)
                {
                    superadmin = new IdentityUser
                    {
                        UserName = "superadmin",
                        Email = "admin@test.com",
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(superadmin, "LtrWR;gF_,U5:6*");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superadmin, "Admin");
                    }
                }
            }
        }
    }
}
