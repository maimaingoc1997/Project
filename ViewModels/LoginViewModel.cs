using System.ComponentModel.DataAnnotations;

namespace CourseShopOnline.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }  // Không sử dụng dynamic ở đây

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }  
}