using CourseShopOnline.Models;

namespace CourseShopOnline.Interfaces;

public interface  IEnrollmentService
{
    Task<Enrollment> GetEnrollmentAsync(string studentId, int courseId);
    
    Task RegisterStudentToCourseAsync(Enrollment enrollment);

    Task<Enrollment> GetEnrollmentByIdAsync(int enrollmentId);            
    Task UpdateEnrollmentStatusAsync(Enrollment enrollment);   
    Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId);
}