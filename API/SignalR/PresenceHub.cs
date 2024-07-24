using Extensions.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;


[Authorize]
public class PresenceHub:Hub
{

    private readonly PresenceTracker _tracker;

    public PresenceHub(PresenceTracker tracker)
    {
        _tracker=tracker;
    }
    public override async Task OnConnectedAsync()
    {
        
        
        var isOnline = await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        if (isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
       
    }

    public override async  Task OnDisconnectedAsync(Exception ex)
   {
      var userName = Context.User?.GetUserName();
        if (userName != null)
        {
            await _tracker.UserDisconnected(userName, Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", userName);

           
        }

        await base.OnDisconnectedAsync(ex);
    }
   }

