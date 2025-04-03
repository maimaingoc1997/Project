namespace CourseShopOnline.Models;

public class TransactionDetail
{
    public int TransactionDetailId { get; set; }

    public int TransactionId { get; set; }
    public Transaction Transaction { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public decimal PriceAtPurchase { get; set; }
}