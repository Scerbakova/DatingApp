namespace API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = [];

    public Task UserConnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (OnlineUsers.TryGetValue(username, out List<string> value))
            {
                value.Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, [connectionId]);
            }
        }
        return Task.CompletedTask;
    }

    public Task UserDisconnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (!OnlineUsers.TryGetValue(username, out List<string> value))
            {
                return Task.CompletedTask;
            }

            OnlineUsers[username].Remove(connectionId);

            if (OnlineUsers[username].Count == 0)
            {
                OnlineUsers.Remove(username);
            }
        }
        return Task.CompletedTask;
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.OrderBy(x => x.Key).Select(k => k.Key).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }
}
