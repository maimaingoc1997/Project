using CourseShopOnline.DataAccess.Context;
using CourseShopOnline.Interfaces;
using CourseShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseShopOnline.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddReviewAsync(Review review)
    {
        bool hasReviewed = await _context.Reviews
            .AnyAsync(r => r.StudentId == review.StudentId && r.CourseId == review.CourseId);

        if (hasReviewed)
            throw new Exception("You have already reviewed this course.");

        review.ReviewDate = DateTime.Now;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasReviewedAsync(string studentId, int courseId)
    {
        return await _context.Reviews
            .AnyAsync(r => r.StudentId == studentId && r.CourseId == courseId);
    }

    public async Task<List<Review>> GetReviewsByCourseIdAsync(int courseId)
    {
        return await _context.Reviews
            .Where(r => r.CourseId == courseId)
            .Include(r => r.Student)
            .ToListAsync();
    }
}
