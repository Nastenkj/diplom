using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace PregnancyAppBackend.Hubs;

public class NotificationHub : Hub
{
    public async Task SendMessage(string userId, string message)
    {
        // Этот метод может использоваться клиентом, если нужно отправить сообщение
        // Для серверной отправки используется NotificationService
        await Clients.All.SendAsync("ReceiveMessage", userId, message);
    }
        
    public override async Task OnConnectedAsync()
    {
        var userIdClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var queryUserId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        
        var userId = !string.IsNullOrEmpty(userIdClaim) ? userIdClaim : queryUserId;
        
        if (!string.IsNullOrEmpty(userId))
        {
            NotificationService.AddConnection(userId, Context.ConnectionId);
        }
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userIdClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var queryUserId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        
        var userId = !string.IsNullOrEmpty(userIdClaim) ? userIdClaim : queryUserId;
        
        if (!string.IsNullOrEmpty(userId))
        {
            NotificationService.RemoveConnection(userId, Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}