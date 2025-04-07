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
   
    Task<IEnumerable<Course>> GetCoursesByTeacherAsync(string teacherId); 
    Task<IEnumerable<Course>> GetApprovedCoursesAsync();        
    Task<List<Course>> GetApprovedCourses();
}
