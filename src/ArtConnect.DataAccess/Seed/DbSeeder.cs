using ArtConnect.Entities;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArtConnect.DataAccess.Seed
{
    public static class DbSeeder
    {
        /// <summary>
        /// Creates the roles, the default admin account and the default categories.
        /// Safe to run on every start: existing data is not duplicated.
        /// The database must already exist (run Update-Database first).
        /// </summary>
        public static async Task SeedAsync(
            IServiceProvider services,
            string adminEmail,
            string adminPassword,
            string adminFullName)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            if (!await context.Database.CanConnectAsync())
            {
                throw new InvalidOperationException(
                    "Cannot connect to the database. Check the connection string and run Update-Database.");
            }

            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Roles
            foreach (var roleName in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await roleManager.CreateAsync(new ApplicationRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Could not create role {roleName}: " +
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            // 2. Default admin
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = adminFullName
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Could not create the default admin: " +
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
            {
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }

            // 3. Default categories
            if (!await context.Categories.AnyAsync())
            {
                var names = new[]
                {
                    "Oil Painting", "Watercolor", "Acrylic Painting", "Digital Art",
                    "Abstract Art", "Portrait Painting", "Landscape Painting", "Calligraphy"
                };

                context.Categories.AddRange(names.Select(n => new Category { Name = n }));
                await context.SaveChangesAsync();
            }
        }
    }
}
