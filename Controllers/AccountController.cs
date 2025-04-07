using System.Security.Claims;
using CourseShopOnline.Models;
using CourseShopOnline.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseShopOnline.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
        IPasswordHasher<User> passwordHasher)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _passwordHasher = passwordHasher;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại.");
                return View(model);
            }

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(user, model.Password);

            var passwordValid = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) ==
                                PasswordVerificationResult.Success;

            if (passwordValid)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không đúng.");
                return View(model);
            }
        }

        return View(model);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (string.IsNullOrEmpty(model.Role))
        {
            ModelState.AddModelError("Role", "Vui lòng chọn vai trò.");
        }

        if (ModelState.ContainsKey("Role"))
        {
            var roleErrors = ModelState["Role"].Errors;
            foreach (var error in roleErrors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
        }

        if (!ModelState.IsValid)
        {
            foreach (var modelError in ModelState.Values)
            {
                foreach (var error in modelError.Errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Log lỗi ra console
                }
            }
        }

        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByNameAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            try
            {
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo tài khoản.");
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account"); 
        }

        var model = new EditProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Error: {error.ErrorMessage}");
            }

            return View(model); 
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        user.FullName = model.FullName;
        user.Email = model.Email;

        var updateResult = await _userManager.UpdateAsync(user);
        if (updateResult.Succeeded)
        {
            TempData["SuccessMessage"] = "Thông tin đã được cập nhật thành công!";
        }
        else
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }


    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); 
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account"); 
        }
        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
            return View(model); 
        }
        var passwordHasher = new PasswordHasher<User>();
        var hashedPassword = passwordHasher.HashPassword(user, model.CurrentPassword);

        var passwordVerificationResult =
            passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword) ==
            PasswordVerificationResult.Success;

        if (!passwordVerificationResult)
        {
            ModelState.AddModelError(string.Empty, "The current password is incorrect.");
            return View(model); 
        }


        var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);


        user.PasswordHash = newPasswordHash;

        var updateResult = await _userManager.UpdateAsync(user);

        if (updateResult.Succeeded)
        {
            TempData["SuccessMessage"] = "Your password has been changed successfully!";
            return
                RedirectToAction("ChangePassword", "Account"); 
        }
        else
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model); 
        }
    }
}