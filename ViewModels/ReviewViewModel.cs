using System.ComponentModel.DataAnnotations;

namespace CourseShopOnline.ViewModels;

public class ReviewViewModel
{
    public int CourseId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    public string Comment { get; set; }
}