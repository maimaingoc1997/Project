using CourseShopOnline.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseShopOnline.ViewModels;

public static class SeedRoles
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Student", "Teacher", "Admin" };

        // Tạo các role nếu chưa có
        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Tạo người dùng Admin nếu chưa có
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var user = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin User"
            };

            var password = "AdminPassword"; // Đảm bảo thay đổi mật khẩu mạnh
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

