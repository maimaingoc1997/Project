using CourseShopOnline.Interfaces;
using CourseShopOnline.Models;
using CourseShopOnline.Models.Enum;
using CourseShopOnline.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseShopOnline.Controllers;

public class CourseController : Controller
{
    // GET
    private readonly ICourseService _courseService;
    private readonly UserManager<User> _userManager;
    private readonly IEnrollmentService _enrollmentService;

    public CourseController(UserManager<User> userManager, ICourseService courseService,
        IEnrollmentService enrollmentService)
    {
        _userManager = userManager;
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (User.IsInRole("Admin"))
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }
        else if (User.IsInRole("Teacher"))
        {
            var teacherId = user.Id;
            var courses = await _courseService.GetCoursesByTeacherAsync(teacherId);
            return View(courses);
        }
        else if (User.IsInRole("Student"))
        {
            var courses = await _courseService.GetApprovedCoursesAsync();
            return View(courses);
        }

        return View(new List<Course>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        var course = new CourseViewModel();
        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseViewModel courseViewModel)
    {
        if (ModelState.IsValid)
        {
            var teacherId = (await _userManager.GetUserAsync(User)).Id;

            var course = new Course
            {
                Title = courseViewModel.Title,
                Description = courseViewModel.Description,
                Price = courseViewModel.Price,
                PreviewVideoUrl = courseViewModel.PreviewVideoUrl,
                ImageUrl = courseViewModel.ImageUrl
            };
            await _courseService.CreateCourseAsync(course, teacherId);

            return RedirectToAction("Index");
        }

        return View(courseViewModel);
    }

    // Sửa khóa học
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        var courseViewModel = new CourseViewModel
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            Price = course.Price,
            PreviewVideoUrl = course.PreviewVideoUrl,
            ImageUrl = course.ImageUrl
        };

        return View(courseViewModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CourseViewModel courseViewModel)
    {
        if (ModelState.IsValid)
        {
            var course = new Course
            {
                CourseId = courseViewModel.CourseId,
                Title = courseViewModel.Title,
                Description = courseViewModel.Description,
                Price = courseViewModel.Price,
                PreviewVideoUrl = courseViewModel.PreviewVideoUrl,
                ImageUrl = courseViewModel.ImageUrl
            };
            await _courseService.UpdateCourseAsync(course);

            return RedirectToAction("Index");
        }

        return View(courseViewModel);
    }


    [HttpGet]
    public async Task<IActionResult> Approve(int id)
    {
        await _courseService.ApproveCourseAsync(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Reject(int id)
    {
        await _courseService.RejectCourseAsync(id);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        await _courseService.DeleteCourseAsync(id);
        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var enrollment = await _enrollmentService.GetEnrollmentAsync(user.Id, course.CourseId);

        var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(course.CourseId);

        var approvedEnrollmentsCount = enrollments.Count(e => e.EnollingStatus == EnollingStatus.Approved);

        ViewData["Enrollment"] = enrollment;
        ViewData["Enrollments"] = enrollments;
        ViewData["ApprovedEnrollmentsCount"] = approvedEnrollmentsCount;

        return View(course);
    }

    [HttpPost]
    public async Task<IActionResult> Enroll(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var enrollment = new Enrollment
        {
            CourseId = course.CourseId,
            StudentId = user.Id,
            Status = LearningStatus.NotStarted, 
            EnrollmentDate = DateTime.Now,
            EnollingStatus = EnollingStatus.Pending 
        };

        var existingEnrollment = await _enrollmentService.GetEnrollmentAsync(user.Id, course.CourseId);
        if (existingEnrollment != null)
        {
            TempData["Message"] = "Bạn đã đăng ký khóa học này rồi.";
            return RedirectToAction("Details", new { id = course.CourseId });
        }

        // Thêm đăng ký mới
        await _enrollmentService.RegisterStudentToCourseAsync(enrollment);
        TempData["Message"] = "Đăng ký khóa học thành công, chờ giảng viên duyệt.";
        return RedirectToAction("Details", new { id = course.CourseId });
    }

    [HttpPost]
    public async Task<IActionResult> ApproveEnrollment(int enrollmentId)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(enrollmentId);
        if (enrollment == null)
        {
            return NotFound();
        }
        enrollment.EnollingStatus = EnollingStatus.Approved;
        await _enrollmentService.UpdateEnrollmentStatusAsync(enrollment);

        TempData["Message"] = "Đã duyệt đăng ký của học sinh.";
        return RedirectToAction("Details", new { id = enrollment.CourseId });
    }
}