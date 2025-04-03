using CourseShopOnline.Models.Enum;
using Microsoft.AspNetCore.Identity;

namespace CourseShopOnline.Models;

public class User : IdentityUser
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Role Role { get; set; }
}