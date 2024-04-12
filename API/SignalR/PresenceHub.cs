using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    private readonly PresenceTracker _tracker = tracker;
    public override async Task OnConnectedAsync()
    {
        await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

        await GetOnlineUsers();
    }
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());
        await GetOnlineUsers();
        await base.OnDisconnectedAsync(exception);
    }

    private async Task GetOnlineUsers()
    {
        string[] currentUsers = await _tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }
}
