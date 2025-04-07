using CourseShopOnline.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseShopOnline.ViewModels;

public static class SeedRoles
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var roleNames = new[] { "Admin", "Teacher", "Student" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
            }
        }

        var adminUser = await userManager.FindByNameAsync("admin@example.com");
        if (adminUser == null)
        {
            var user = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FullName = "Admin User"
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Admin123!"); 
            var result = await userManager.CreateAsync(user, "Admin123!"); 

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error creating admin user: {error.Description}");
                }
            }
        }
    }
}