using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ChatService.Hubs;
using ChatService.Repositories;

namespace ChatService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {

        private readonly IHubContext<ChatHub> _hubContext;
        private readonly MessageRepository _messageRepository;

        public RoomController(IHubContext<ChatHub> hubContext, MessageRepository messageRepository)
        {
            _hubContext = hubContext;
            _messageRepository = messageRepository;
        }

        // GET: api/chat/
        [HttpGet()]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var chatService = new ChatService.Services.ChatService();
            chatService.SendMessageToRoomService("A new user has joined the room");
            return Ok("works");
        }

        // POST: api/chat/send
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] string message)
        {
            // Hier sendest du die Nachricht an alle verbundenen Clients
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Server", message);

            return Ok(new { Message = "Notification sent!" });
        }

        // GET: api/chat/{roomId}/messages 
        [HttpGet("/{roomId}/messages")]
        public async Task<IActionResult> GetMessagesByRoomId(int roomId)
        {
            var messages = await _messageRepository.GetMessagesByRoomIdAsync(roomId);

            return Ok(messages);
        }
    }
}