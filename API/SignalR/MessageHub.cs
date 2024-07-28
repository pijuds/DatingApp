using System.Linq;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Extensions.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class MessageHub:Hub
{

    
    private readonly IuintofWork _uow;
    private readonly IMapper _mapper;

    private readonly IHubContext<PresenceHub> _presencehub;
    public MessageHub(IuintofWork uow,
    IMapper mapper,IHubContext<PresenceHub> presencehub)
    {
       
        _mapper=mapper;
        _presencehub=presencehub;
        _uow=uow;

    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"];
        var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdateGroup",group);
        if(_uow.HasChanges()) await _uow.Complete();
        var messages = await _uow.MessageRepository
            .GetMessageThread(Context.User.GetUserName(), otherUser);
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

   

     public async Task SendMessage(CreateMessageDto createMessageDto)
     {
        var username = Context.User.GetUserName();

        if (username == createMessageDto.RecipientUsername.ToLower())
        {
            throw new HubException("You cannot send messages to yourself");
        }

        var sender = await _uow.UserRepository.GetUserByNameAsync(username);
        var recipient = await _uow.UserRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

        if (recipient == null)
        {
            throw new HubException("Not found user");
        }

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName=GetGroupName(sender.UserName,recipient.UserName);
        var group= await _uow.MessageRepository.GetMessageGroup(groupName);
       


        if(group.Connections.Any(x=>x.Username==recipient.UserName))
        {
            message.DateRead=DateTime.UtcNow;
        }
        else
        {
            var connections=await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if(connections !=null)
            {
                await _presencehub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                    new { username = sender.UserName, knownAs = sender.KnownAs });
            }
        }

        _uow.MessageRepository.AddMessage(message);

        if (await _uow.Complete())
        {
            
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
         else
         {
          throw new HubException("Failed to save message");
    }
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
    
    public override async Task OnDisconnectedAsync(Exception ex)
    {
        
        var group=await  RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdateGroup");
        await base.OnDisconnectedAsync(ex);
    }

    private async Task<Group> AddToGroup(string groupName)
    {
           var group=await _uow.MessageRepository.GetMessageGroup(groupName);
           var connection=new Connection(Context.ConnectionId,Context.User.GetUserName());

           if(group ==null)
           {
             group=new Group(groupName);
             _uow.MessageRepository.AddGroup(group);
           }
           group.Connections.Add(connection);
           if(await _uow.Complete()) return group;

        throw new HubException("Failed to add to group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group=await _uow.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection=group.Connections.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);
        _uow.MessageRepository.RemoveConnection(connection);

        if(await _uow.Complete()) return group;

        throw new HubException("Failed to remove from group");
    }

}