using ChatService.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace RoomService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {

        private readonly IHubContext<ChatHub> _hubContext;

        public RoomController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
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
    }
}