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

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IPasswordHasher<User> passwordHasher)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _passwordHasher = passwordHasher;
    }
    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
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
            
            var passwordValid = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success;

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
    
    // GET: /Account/Register
    public IActionResult Register()
    {
        var model = new RegisterViewModel
        {
            // Không bao gồm Admin trong dropdown
            Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "Student", Value = "Student" },
                new SelectListItem { Text = "Teacher", Value = "Teacher" }
            }
        };
        return View(model);
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
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
                    // Gán role cho người dùng là Student hoặc Teacher
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

        // Lấy lại danh sách role để hiển thị trong dropdown khi có lỗi
        model.Roles = new List<SelectListItem>
        {
            new SelectListItem { Text = "Student", Value = "Student" },
            new SelectListItem { Text = "Teacher", Value = "Teacher" }
        };
        return View(model);
    }
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
