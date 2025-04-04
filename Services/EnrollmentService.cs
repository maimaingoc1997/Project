using CourseShopOnline.DataAccess.Context;
using CourseShopOnline.Interfaces;
using CourseShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseShopOnline.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;

    public EnrollmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Kiểm tra xem học sinh đã đăng ký khóa học chưa
    public async Task<Enrollment> GetEnrollmentAsync(string studentId, int courseId)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    // Đăng ký học sinh vào khóa học
    public async Task RegisterStudentToCourseAsync(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }
    

    // Lấy thông tin đăng ký theo ID
    public async Task<Enrollment> GetEnrollmentByIdAsync(int enrollmentId)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);
    }

    // Cập nhật trạng thái đăng ký (duyệt hoặc từ chối)
    public async Task UpdateEnrollmentStatusAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId)
    {
        return await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.Student)  // Bao gồm thông tin học sinh
            .Include(e => e.Course)   // Bao gồm thông tin khóa học
            .ToListAsync();
    }
}
