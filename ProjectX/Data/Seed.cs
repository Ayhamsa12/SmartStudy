using Microsoft.AspNetCore.Identity;
using ProjectX.Models;

namespace ProjectX.Data
{
    public static class Seed
    {
        public static void SeedUsersAndRoles(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<Users>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Seed roles
                SeedRoles(roleManager);

                // Seed admin user
                SeedAdminUser(userManager);

                SeedRegularUser(userManager);
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(UserRoles.Admin).Result)
            {
                var adminRole = new IdentityRole(UserRoles.Admin);
                roleManager.CreateAsync(adminRole).Wait();
            }

            if (!roleManager.RoleExistsAsync(UserRoles.User).Result)
            {
                var userRole = new IdentityRole(UserRoles.User);
                roleManager.CreateAsync(userRole).Wait();
            }
        }

        private static void SeedAdminUser(UserManager<Users> userManager)
        {
            var adminUser = userManager.FindByNameAsync("admin").Result;
            if (adminUser == null)
            {
                var user = new Users
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "Admin@123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, UserRoles.Admin).Wait();
                }
            }

        }
        private static void SeedRegularUser(UserManager<Users> userManager)
        {
            var regularUser = userManager.FindByNameAsync("user").Result;
            if (regularUser == null)
            {
                var user = new Users
                {
                    UserName = "user",
                    Email = "user@example.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "User@123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, UserRoles.User).Wait();
                }
            }
        }
    }
}



