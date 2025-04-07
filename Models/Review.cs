using System.ComponentModel.DataAnnotations;

namespace CourseShopOnline.Models;

public class Review
{
    public int ReviewId { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public string StudentId { get; set; }
    public User Student { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; } // 1 to 5
    public string Comment { get; set; }
    public DateTime ReviewDate { get; set; }
}