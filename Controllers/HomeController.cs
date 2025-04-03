using CourseShopOnline.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseShopOnline.Controllers;

[AllowAnonymous]
public class HomeController: Controller
{
    private readonly ICourseService _courseService;
    private readonly INotificationService _notificationService; // Dịch vụ thông báo (ví dụ, nếu muốn gửi thông báo cho người dùng)

    public HomeController(ICourseService courseService, INotificationService notificationService)
    {
        _courseService = courseService;
        _notificationService = notificationService;
    }

    // Trang Home chung cho tất cả người dùng (đã đăng nhập và chưa đăng nhập)
    public ActionResult Index()
    {
        return View();
    }

     
}
