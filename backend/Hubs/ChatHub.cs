using Microsoft.AspNetCore.SignalR;

namespace learnyx.Hubs;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, (string User, DateTime LastSeen)> _users = new();

    public override async Task OnConnectedAsync()
    {
        var user = Context.User?.Identity?.Name ?? Context.ConnectionId;
        _users[Context.ConnectionId] = (user, DateTime.UtcNow);

        await Clients.All.SendAsync("UserOnline", user);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_users.Remove(Context.ConnectionId, out var userInfo))
        {
            var lastSeen = DateTime.UtcNow;

            await Clients.All.SendAsync("UserOffline", userInfo.User, lastSeen);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string user, string message)
    {
        // save a message to DB here

        // broadcast
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
