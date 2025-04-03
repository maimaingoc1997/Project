using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseShopOnline.ViewModels;

public class RegisterViewModel
{
    [Required]
    [StringLength(100, ErrorMessage = "Tên đầy đủ phải ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 2)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
    public string ConfirmPassword { get; set; }
    
    [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
    public string Role { get; set; }

    // Để sử dụng dropdown trong view
    
}