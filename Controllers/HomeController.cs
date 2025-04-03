using Microsoft.AspNetCore.Mvc;

namespace CourseShopOnline.Controllers;

public class HomeController: Controller
{
    public IActionResult Index()
    {
       
        return View();
    }
}