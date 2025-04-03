namespace CourseShopOnline.Models;

public class CartItem
{
    public int CartItemId { get; set; }

    public string StudentId { get; set; }
    public User Student { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public DateTime AddedDate { get; set; }
}