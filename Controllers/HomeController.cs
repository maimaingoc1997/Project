using CourseShopOnline.Interfaces;
using CourseShopOnline.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseShopOnline.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ICourseService _courseService;

    public HomeController(UserManager<User> userManager, ICourseService courseService)
    {
        _userManager = userManager;
        _courseService = courseService;
    }

    public async Task<ActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Student"))
        {
            ViewBag.StudentCourses = _courseService.GetStudentCourses(user.UserName);
        }
        else if (roles.Contains("Teacher"))
        {
            ViewBag.TeacherCourses = _courseService.GetTeacherCourses(user.UserName);
        }
        else if (roles.Contains("Admin"))
        {
            ViewBag.PendingCourses = _courseService.GetPendingCourses();
        }

        var approvedCourses = await _courseService.GetApprovedCourses();

        return View(approvedCourses);
    }

}