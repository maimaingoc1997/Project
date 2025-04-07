using CourseShopOnline.DataAccess.Context;
using CourseShopOnline.Interfaces;
using CourseShopOnline.Models;
using CourseShopOnline.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseShopOnline.Controllers;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly UserManager<User> _userManager;

    public ReviewController(IReviewService reviewService, UserManager<User> userManager)
    {
        _reviewService = reviewService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create(int courseId)
    {
        return View(new ReviewViewModel { CourseId = courseId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReviewViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var student = await _userManager.GetUserAsync(User);
        if (student == null)
            return Unauthorized();

        bool alreadyReviewed = await _reviewService.HasReviewedAsync(student.Id, model.CourseId);
        if (alreadyReviewed)
        {
            ModelState.AddModelError("", "Bạn đã đánh giá khóa học này rồi.");
            return View(model);
        }

        var review = new Review
        {
            CourseId = model.CourseId,
            StudentId = student.Id,
            Rating = model.Rating,
            Comment = model.Comment
        };

        await _reviewService.AddReviewAsync(review);

        return RedirectToAction("CourseReviews");
    }

    
    public async Task<IActionResult> CourseReviews(int courseId)
    {
        var reviews = await _reviewService.GetReviewsByCourseIdAsync(courseId);
        return View(reviews);
    }

}