using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace API.SignalR;

[Authorize]
public class MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper) : Hub
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public override async Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext();

        StringValues otherUser = httpContext.Request.Query["user"];

        string groupName = GetGroupName(Context.User.GetUserName(), otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        IEnumerable<MessageDto> messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        string username = Context.User.GetUserName();

        if (username.Equals(createMessageDto.RecipientUsername, StringComparison.CurrentCultureIgnoreCase))
            throw new HubException("You cannot send messages to yourself");

        AppUser sender = await _userRepository.GetUserByUserNameAsync(username);
        AppUser recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername) ?? throw new HubException("User not found");
        Message message = new()
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            string group = GetGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
        throw new HubException("Failed to save message");
    }

    private static string GetGroupName(string caller, string other)
    {
        bool stringCompare = string.CompareOrdinal(caller, other) < 0;

        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

}
