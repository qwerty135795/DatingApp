using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IMessageRepository messageRepository
        ,IUserRepository userRepository,IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.Request.Query["user"];

            var groupName = GetGroupName(Context.User.GetUsername(), user);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup",group);
            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), user);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }
        public async Task SendMessage(CreateMessageDTO messageDTO)
        {
            var senderUsername = Context.User.GetUsername();

            if (senderUsername == messageDTO.RecipientUsername.ToLower()) 
                throw new HubException("You cannot send message yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(senderUsername);
            var recipient = await _userRepository.GetUserByUsernameAsync(messageDTO.RecipientUsername);

            if (recipient is null) throw new HubException("User not found");


            var message = new Message
            {
                Sender = sender,
                SenderUsername = sender.UserName,
                Recipient = recipient,
                RecipientUsername = recipient.UserName,
                Content = messageDTO.Content
            };
            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetGroup(groupName);
            if(group.Connections.Any(g => g.UserName == recipient.UserName)){
                message.DateRead = DateTime.UtcNow;
            }
            else {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if(connections != null) {
                    await _presenceHub.Clients.Clients(connections)
                    .SendAsync("NewMessageReceived",
                     new {username = sender.UserName,knownAs = sender.KnownAs});
                }
            }
            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) {
                await Clients.Group(groupName).SendAsync("NewMessage",
                _mapper.Map<MessageDTO>(message));
            }
            
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup",group);
            await base.OnDisconnectedAsync(exception);
        }
        private async Task<Group> AddToGroup(string groupName) {
            var group = await _messageRepository.GetGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(group is null) {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
            if(await _messageRepository.SaveAllAsync()) return group;
            throw new HubException("failed to add connection in group");
        }
        private async Task<Group> RemoveFromMessageGroup() {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(c => 
            c.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            if(await _messageRepository.SaveAllAsync()) return group;
            throw new HubException("Fail to remove connection");
        }
        private string GetGroupName(string caller, string other)
        {
            var compareString = string.CompareOrdinal(caller, other) < 0;
            return compareString ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}