using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        readonly IMessageRepository _messageRepository;
        readonly IMapper _mapper;
        readonly IUserRepository _userRepository;
        readonly IHubContext<PresenceHub> _presenceHub;
        readonly PresenceTracker _tracker;
        public MessageHub(IMessageRepository messageRepository,
                          IUserRepository userRepository,
                          IMapper mapper,
                          IHubContext<PresenceHub> presenceHub,
                          PresenceTracker tracker)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _presenceHub = presenceHub;
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUserName.ToLower())
                throw new HubException("You cannot send messages for yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUserName);

            if (recipient == null) throw new HubException("Not found user");

            var message = new Message
            {
                UserSender = sender,
                UserRecipient = recipient,
                UserSenderUsername = sender.UserName,
                UserRecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(con => con.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await _tracker.GetConnectionForUser(recipient.UserName);

                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections)
                                              .SendAsync("NewMessageReceived",
                                                          new
                                                          {
                                                              username = sender.UserName,
                                                              knownAs = sender.KnownAs
                                                          });
                }
            }


            _messageRepository.AddMessage(message);
            if (await _messageRepository.SalveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
            else
            {
                throw new HubException("Failed to send message");
            }

        }


        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _messageRepository.SalveAllAsync()) return group;

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(cnn => cnn.ConnectionId == Context.ConnectionId);

            _messageRepository.RemoveConnection(connection);

            if (await _messageRepository.SalveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
        }



        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}