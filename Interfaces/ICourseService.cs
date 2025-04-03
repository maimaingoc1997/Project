using CourseShopOnline.Models;

namespace CourseShopOnline.Interfaces;

public interface ICourseService
{
    // Lấy danh sách khóa học đã duyệt
    IEnumerable<Course> GetApprovedCourses();
    
    // Lấy danh sách khóa học của sinh viên (dựa trên tên đăng nhập)
    IEnumerable<Course> GetStudentCourses(string studentUsername);
    
    // Lấy danh sách khóa học của giảng viên (dựa trên tên đăng nhập)
    IEnumerable<Course> GetTeacherCourses(string teacherUsername);
    
    // Lấy tất cả khóa học (admin)
    IEnumerable<Course> GetAllCourses();
}