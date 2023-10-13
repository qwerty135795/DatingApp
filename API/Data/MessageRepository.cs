using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }
        public  void AddMessage(Message message)
        {
             _context.Messages.Add(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessages(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch 
            {
                "Inbox" => query.Where(m => m.RecipientUsername == messageParams.Username && !m.RecipientDeleted),
                "Outbox" => query.Where(m => m.SenderUsername == messageParams.Username && !m.SenderDeleted),
                _ => query.Where(m =>
                 m.RecipientUsername == messageParams.Username && !m.RecipientDeleted && m.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDTO>.CreateAsync(messages,
            messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string senderUsername, string recipientUsername)
        {
            var messages = await _context.Messages.Include(m => m.Sender)
            .ThenInclude(u => u.Photos).Include(m => m.Recipient)
            .ThenInclude(u => u.Photos).Where(m => 
            m.SenderUsername == senderUsername && m.RecipientUsername == recipientUsername && !m.SenderDeleted
            || m.RecipientUsername == senderUsername && m.SenderUsername == recipientUsername && !m.RecipientDeleted
            )
            .OrderBy(m => m.MessageSent).ToListAsync();
            var unreadMessage = messages.Where(m => m.DateRead == null && m.SenderUsername != senderUsername).ToList();
            if(unreadMessage.Any()) {
                foreach(var message in unreadMessage) {
                    message.DateRead = DateTime.UtcNow;
                }
            }
            await _context.SaveChangesAsync();
            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public void RemoveMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}