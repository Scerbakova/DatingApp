﻿using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace API.SignalR;

[Authorize]
public class MessageHub(
IUnitOfWork uow,
IMapper mapper,
IHubContext<PresenceHub> presenceHub) : Hub
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;
    private readonly IHubContext<PresenceHub> _presenceHub = presenceHub;

    public override async Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext();

        StringValues otherUser = httpContext.Request.Query["user"];

        string groupName = GetGroupName(Context.User.GetUserName(), otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        Group group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        IEnumerable<MessageDto> messages = await _uow.MessageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);
        if (_uow.HasChanges()) await _uow.Complete();

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Group group = await RemoveFromMessageGroup();

        await Clients.Group(group.Name).SendAsync("UpdatedGroup");
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        string username = Context.User.GetUserName();

        if (username.Equals(createMessageDto.RecipientUsername, StringComparison.CurrentCultureIgnoreCase))
            throw new HubException("You cannot send messages to yourself");

        AppUser sender = await _uow.UserRepository.GetUserByUserNameAsync(username);
        AppUser recipient = await _uow.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername) ?? throw new HubException("User not found");
        Message message = new()
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        string groupName = GetGroupName(sender.UserName, recipient.UserName);

        Group group = await _uow.MessageRepository.GetMessageGroup(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUsers(recipient.UserName);
            if (connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new { username = sender.UserName, knownAs = sender.KnownAs });
            }
        }

        _uow.MessageRepository.AddMessage(message);

        if (await _uow.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
        throw new HubException("Failed to save message");
    }

    private static string GetGroupName(string caller, string other)
    {
        bool stringCompare = string.CompareOrdinal(caller, other) < 0;

        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        Group group = await _uow.MessageRepository.GetMessageGroup(groupName);
        Connection connection = new(Context.ConnectionId, Context.User.GetUserName());

        if (group == null)
        {
            group = new(groupName);
            _uow.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await _uow.Complete()) return group;

        throw new HubException("Failed to add to group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        Group group = await _uow.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        Connection connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

        _uow.MessageRepository.RemoveConnection(connection);

        if (await _uow.Complete()) return group;

        throw new HubException("Failed to remove from group");
    }

}
