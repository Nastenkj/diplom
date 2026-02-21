namespace PregnancyAppBackend.Hubs;

public interface INotificationService
{
    Task SendNotification(string user, string message);
}