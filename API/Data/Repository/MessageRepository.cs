using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository
{
    public class MessageRepository : IMessageRepository
    {

        readonly DataContext _context;
        readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                                          .Include(user => user.UserSender)
                                          .Include(user => user.UserRecipient)
                                          .SingleOrDefaultAsync(message => message.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(messages => messages.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(message => message.UserRecipient.UserName == messageParams.Username 
                                               && message.RecipientDeleted == false),

                "Outbox" => query.Where(message => message.UserSender.UserName == messageParams.Username
                                                && message.SenderDeleted == false),

                _ => query.Where(message => message.UserRecipient.UserName == messageParams.Username 
                                         && message.DateRead == null
                                         && message.RecipientDeleted == false)

            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                                            .Include(message => message.UserSender).ThenInclude(photos => photos.Photos)
                                            .Include(message => message.UserRecipient).ThenInclude(photos => photos.Photos)
                                            .Where(message => message.UserRecipientUsername == currentUsername 
                                                            && message.UserSenderUsername == recipientUsername
                                                            && message.RecipientDeleted == false
                                                            ||
                                                               message.UserRecipientUsername == recipientUsername
                                                            && message.UserSenderUsername == currentUsername
                                                            && message.SenderDeleted == false)

                                            .OrderBy(m => m.MessageSent)
                                            .ToListAsync();

            var unreadMessages = messages
                                        .Where(messages => messages.DateRead == null
                                                        && messages.UserRecipientUsername == currentUsername)
                                        .ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;

                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> SalveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}