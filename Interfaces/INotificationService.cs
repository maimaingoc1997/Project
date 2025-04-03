namespace CourseShopOnline.Interfaces;

public interface INotificationService
{
    // Gửi thông báo lỗi
    void SendErrorNotification(string message);

    // Gửi thông báo thông thường
    void SendNotification(string message);
}