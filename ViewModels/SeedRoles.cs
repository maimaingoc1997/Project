using CourseShopOnline.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseShopOnline.ViewModels;

public static class SeedRoles
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var roleNames = new[] { "Admin", "Teacher", "Student" };

        // Tạo các vai trò nếu chưa có
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
            }
        }

        // Tạo người dùng Admin nếu chưa có
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
            user.PasswordHash = passwordHasher.HashPassword(user, "Admin123!"); // Mật khẩu cho Admin
            var result = await userManager.CreateAsync(user, "Admin123!"); // Tạo người dùng với mật khẩu đã mã hóa

            if (result.Succeeded)
            {
                // Gán vai trò Admin cho người dùng
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                // Log lỗi nếu tạo người dùng Admin không thành công
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error creating admin user: {error.Description}");
                }
            }
        }
    }
}