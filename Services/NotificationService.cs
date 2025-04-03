using CourseShopOnline.Interfaces;

namespace CourseShopOnline.Services;

public class NotificationService : INotificationService
{
    public void SendErrorNotification(string message)
    {
        // Logic gửi thông báo lỗi (Ví dụ: Ghi log vào file, gửi email, hoặc lưu vào database)
        Console.WriteLine("Error: " + message); // Placeholder, có thể thay bằng ghi log hoặc gửi email thực tế
    }

    public void SendNotification(string message)
    {
        // Logic gửi thông báo thông thường
        Console.WriteLine("Notification: " + message); // Placeholder
    }
}
