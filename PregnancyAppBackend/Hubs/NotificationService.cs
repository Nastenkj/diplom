using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace PregnancyAppBackend.Hubs;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private static readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public static void AddConnection(string userId, string connectionId)
    {
        _connections.AddOrUpdate(
            userId,
            new HashSet<string> { connectionId },
            (_, connections) => 
            {
                connections.Add(connectionId);
                return connections;
            });
    }

    public static void RemoveConnection(string userId, string connectionId)
    {
        if (_connections.TryGetValue(userId, out var connections))
        {
            connections.Remove(connectionId);
            
            if (connections.Count == 0)
            {
                _connections.TryRemove(userId, out _);
            }
        }
    }

    public async Task SendNotification(string userId, string message)
    {
        if (_connections.TryGetValue(userId, out var connectionIds))
        {
            foreach (var connectionId in connectionIds)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", userId, message);
            }
        }
    }
}