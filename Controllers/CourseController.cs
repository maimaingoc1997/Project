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

    public CourseController(UserManager<User> userManager, ICourseService courseService, IEnrollmentService enrollmentService)
    {
        _userManager = userManager;
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

        // Hiển thị danh sách khóa học
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Lấy thông tin người dùng hiện tại

            if (User.IsInRole("Admin"))
            {
                // Admin sẽ thấy tất cả các khóa học
                var courses = await _courseService.GetAllCoursesAsync();
                return View(courses);
            }
            else if (User.IsInRole("Teacher"))
            {
                // Teacher sẽ chỉ thấy khóa học của chính mình
                var teacherId = user.Id;
                var courses = await _courseService.GetCoursesByTeacherAsync(teacherId);
                return View(courses);
            }
            else if (User.IsInRole("Student"))
            {
                // Student chỉ thấy các khóa học đã được duyệt
                var courses = await _courseService.GetApprovedCoursesAsync();
                return View(courses);
            }

            return View(new List<Course>()); // Trả về danh sách trống nếu không phải Admin, Teacher hoặc Student
        }

        // Thêm khóa học mới
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
                // Lấy ID của giảng viên từ người dùng đang đăng nhập
                var teacherId = (await _userManager.GetUserAsync(User)).Id;

                // Tạo khóa học từ ViewModel
                var course = new Course
                {
                    Title = courseViewModel.Title,
                    Description = courseViewModel.Description,
                    Price = courseViewModel.Price,
                    PreviewVideoUrl = courseViewModel.PreviewVideoUrl
                };

                // Gọi service bất đồng bộ để thêm khóa học
                await _courseService.CreateCourseAsync(course, teacherId);

                return RedirectToAction("Index"); // Redirect đến trang danh sách khóa học
            }

            return View(courseViewModel); // Trả về View với dữ liệu đã nhập
        }

        // Sửa khóa học
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Lấy thông tin khóa học từ cơ sở dữ liệu
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound(); // Nếu không tìm thấy khóa học, trả về 404
            }

            // Chuyển đổi từ Course sang CourseViewModel nếu cần (tùy vào View yêu cầu kiểu nào)
            var courseViewModel = new CourseViewModel
            {  CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                PreviewVideoUrl = course.PreviewVideoUrl
            };

            return View(courseViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                // Chuyển CourseViewModel sang Course để lưu vào cơ sở dữ liệu
                var course = new Course
                {   
                    CourseId = courseViewModel.CourseId,
                    Title = courseViewModel.Title,
                    Description = courseViewModel.Description,
                    Price = courseViewModel.Price,
                    PreviewVideoUrl = courseViewModel.PreviewVideoUrl
                };

                // Gọi service để cập nhật khóa học
                await _courseService.UpdateCourseAsync(course);

                return RedirectToAction("Index"); // Redirect đến trang danh sách khóa học sau khi cập nhật thành công
            }

            return View(courseViewModel); // Trả về view nếu model không hợp lệ
        }


        // Duyệt khóa học (Admin)
        
        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            await _courseService.ApproveCourseAsync(id);
            return RedirectToAction("Index");
        }

        // Từ chối khóa học (Admin)
       
        [HttpGet]
        public async Task<IActionResult> Reject(int id)
        {
            await _courseService.RejectCourseAsync(id);
            return RedirectToAction("Index");
        }

        // Xóa khóa học
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

            // Lấy danh sách học sinh đã đăng ký khóa học
            var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(course.CourseId);

            // Tính toán số lượng học sinh đã đăng ký (chỉ học sinh đã duyệt)
            var approvedEnrollmentsCount = enrollments.Count(e => e.EnollingStatus == EnollingStatus.Approved);

            // Truyền thông tin khóa học, đăng ký của học sinh, danh sách đăng ký và số lượng học sinh đã đăng ký vào View
            ViewData["Enrollment"] = enrollment;
            ViewData["Enrollments"] = enrollments;
            ViewData["ApprovedEnrollmentsCount"] = approvedEnrollmentsCount;

            return View(course);
        }

    // POST: Course/Enroll/{id} - Học sinh đăng ký khóa học
    [HttpPost]

    public async Task<IActionResult> Enroll(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User); // Lấy thông tin học sinh
        var enrollment = new Enrollment
        {
            CourseId = course.CourseId,
            StudentId = user.Id,
            Status = LearningStatus.NotStarted, // Trạng thái học ban đầu
            EnrollmentDate = DateTime.Now,
            EnollingStatus = EnollingStatus.Pending // Trạng thái đăng ký là "Chờ duyệt"
        };

        // Kiểm tra xem học sinh đã đăng ký khóa học chưa
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

    // Giảng viên duyệt đăng ký của học sinh
    [HttpPost]

    public async Task<IActionResult> ApproveEnrollment(int enrollmentId)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(enrollmentId);
        if (enrollment == null)
        {
            return NotFound();
        }

        // Cập nhật trạng thái đăng ký thành "Approved"
        enrollment.EnollingStatus = EnollingStatus.Approved;
        await _enrollmentService.UpdateEnrollmentStatusAsync(enrollment);

        TempData["Message"] = "Đã duyệt đăng ký của học sinh.";
        return RedirectToAction("Details", new { id = enrollment.CourseId });
    }
}
    
