using CourseShopOnline.Models;

namespace CourseShopOnline.Interfaces;

public interface IReviewService
{
    Task AddReviewAsync(Review review);
    Task<bool> HasReviewedAsync(string studentId, int courseId);
    Task<List<Review>> GetReviewsByCourseIdAsync(int courseId);
}
