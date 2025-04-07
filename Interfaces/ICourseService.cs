using CourseShopOnline.Models;

namespace CourseShopOnline.Interfaces;

public interface ICourseService
{
    List<Course> GetPendingCourses();
    List<Course> GetTeacherCourses(string username);
    List<Course> GetStudentCourses(string username);
    
    Task<List<Course>> GetAllCoursesAsync();
    Task<Course> GetCourseByIdAsync(int courseId);
    Task UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(int courseId);
    Task ApproveCourseAsync(int courseId);
    Task RejectCourseAsync(int courseId);
    Task CreateCourseAsync(Course course, string teacherId);
   
    Task<IEnumerable<Course>> GetCoursesByTeacherAsync(string teacherId); // Lấy khóa học của giảng viên
    Task<IEnumerable<Course>> GetApprovedCoursesAsync();        // Lấy khóa học đã duyệt
    Task<List<Course>> GetApprovedCourses();
}
