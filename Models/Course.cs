using CourseShopOnline.Models.Enum;

namespace CourseShopOnline.Models;

public class Course
{
    public int CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PreviewVideoUrl { get; set; }
    public CourseStatus Status { get; set; }

    public string TeacherId { get; set; }
    public User Teacher { get; set; }
    public string ImageUrl { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<Review> Reviews { get; set; }
}