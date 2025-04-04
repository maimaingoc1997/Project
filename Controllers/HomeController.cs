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
        // Lấy thông tin người dùng hiện tại
        var user = await _userManager.GetUserAsync(User);

        // Kiểm tra xem người dùng có đăng nhập hay không
        if (user == null)
        {
            // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập hoặc thông báo lỗi
            return RedirectToAction("Login", "Account");
        }

        // Lấy vai trò của người dùng
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Student"))
        {
            // Nếu là student, hiển thị các khóa học đang học và đã hoàn thành
            ViewBag.StudentCourses = _courseService.GetStudentCourses(user.UserName);
        }
        else if (roles.Contains("Teacher"))
        {
            // Nếu là teacher, hiển thị màn quản lý khóa học
            ViewBag.TeacherCourses = _courseService.GetTeacherCourses(user.UserName);
        }
        else if (roles.Contains("Admin"))
        {
            // Nếu là admin, hiển thị màn duyệt khóa học
            ViewBag.PendingCourses = _courseService.GetPendingCourses();
        }

        return View();
    }
}