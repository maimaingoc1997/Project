namespace CourseShopOnline.ViewModels;

public class CourseViewModel
{
    public int CourseId { get; set; } 
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PreviewVideoUrl { get; set; }
}