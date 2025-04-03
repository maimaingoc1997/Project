namespace CourseShopOnline.Models;

public class Transaction
{
    public int TransactionId { get; set; }

    public string StudentId { get; set; }
    public User Student { get; set; }

    public decimal TotalAmount { get; set; }
    public DateTime TransactionDate { get; set; }

    public ICollection<TransactionDetail> Details { get; set; }
}