using CourseShopOnline.DataAccess.Context;
using CourseShopOnline.Interfaces;
using CourseShopOnline.Models;
using CourseShopOnline.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace CourseShopOnline.Services;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lấy danh sách khóa học của student
    public List<Course> GetStudentCourses(string studentId)
    {
        // Truy vấn danh sách các khóa học mà sinh viên đã đăng ký
        var courses = _context.Enrollments
            .Where(e => e.StudentId == studentId && e.Status != LearningStatus.NotStarted)
            .Select(e => e.Course)
            .ToList();

        return courses;
    }

    // Các phương thức khác
    public List<Course> GetTeacherCourses(string username)
    {
        return _context.Courses
            .Where(c => c.TeacherId == username)
            .ToList();
    }

    public List<Course> GetPendingCourses()
    {
        return _context.Courses
            .Where(c => c.Status == CourseStatus.PendingApproval)
            .ToList();
    }
     

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        return await _context.Courses.Include(c => c.Teacher).ToListAsync();
    }

    public async Task<Course> GetCourseByIdAsync(int courseId)
    {
        return await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
    }

    public async Task CreateCourseAsync(Course course, string teacherId)
    {
        // Set thông tin giảng viên và trạng thái khóa học
        course.TeacherId = teacherId;
        course.Status = CourseStatus.PendingApproval;

        // Thêm khóa học vào cơ sở dữ liệu
        await _context.Courses.AddAsync(course);
        
        // Lưu thay đổi vào cơ sở dữ liệu (bất đồng bộ)
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCourseAsync(Course course)
    {
        
        var existingCourse = await _context.Courses.FindAsync(course.CourseId);

        if (existingCourse != null)
        {
            // Cập nhật các thuộc tính của khóa học
            existingCourse.Title = course.Title;
            existingCourse.Description = course.Description;
            existingCourse.Price = course.Price;
            existingCourse.PreviewVideoUrl = course.PreviewVideoUrl;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
        }
        else
        {
            // Nếu không tìm thấy khóa học, có thể ném ra ngoại lệ hoặc làm gì đó tùy ý
            throw new InvalidOperationException("Course not found.");
        }
    }


    public async Task DeleteCourseAsync(int courseId)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ApproveCourseAsync(int courseId)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course != null && course.Status == CourseStatus.PendingApproval)
        {
            course.Status = CourseStatus.Approved;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RejectCourseAsync(int courseId)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        if (course != null && course.Status == CourseStatus.PendingApproval)
        {
            course.Status = CourseStatus.Rejected;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<IEnumerable<Course>> GetCoursesByTeacherAsync(string teacherId)
    {
        return await _context.Courses
            .Where(c => c.TeacherId == teacherId)
            .Include(c => c.Teacher)
            .ToListAsync();
    }
    public async Task<IEnumerable<Course>> GetApprovedCoursesAsync()
    {
        return await _context.Courses
            .Where(c => c.Status == CourseStatus.Approved)
            .Include(c => c.Teacher)
            .ToListAsync();
    }
}