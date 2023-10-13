using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository
        , IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO messageDTO)
        {
            var senderUsername = User.GetUsername();

            if(senderUsername == messageDTO.RecipientUsername.ToLower())  return BadRequest("You cannot sent message yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(senderUsername);
            var recipient = await _userRepository.GetUserByUsernameAsync(messageDTO.RecipientUsername);

            if(recipient is null) return  NotFound();

            var message = new Message {
                Sender = sender,
                SenderUsername = sender.UserName,
                Recipient = recipient,
                RecipientUsername = recipient.UserName,
                Content = messageDTO.Content
            };
            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to send message");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery]MessageParams messageParams) 
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageRepository.GetMessages(messageParams);
            Response.AddPaginationHeaders(
                new PaginationHeaders(messages.CurrentPage,messages.PageSize, messages.TotalCount,
                messages.TotalPage));
            return Ok(messages);
        }
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username) {
            string currentUsername = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id) {
            var username = User.GetUsername();

            var message = await _messageRepository.GetMessage(id);

            if(message.SenderUsername != username && message.RecipientUsername != username ) return Unauthorized();

            if(message.SenderUsername == username) message.SenderDeleted= true;
            if(message.RecipientUsername == username) message.RecipientDeleted = true;
            if(message.RecipientDeleted && message.SenderDeleted) {
                _messageRepository.RemoveMessage(message);
            }
            if(await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}