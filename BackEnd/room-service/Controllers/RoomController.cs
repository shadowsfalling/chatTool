using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RoomService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly RoomDbContext _context;
        private readonly Services.RoomService _roomService;

        public RoomController(RoomDbContext context, Services.RoomService roomService)
        {
            _context = context;
            _roomService = roomService;
        }

        // POST: api/room
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto roomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Überprüfe, ob der Raumname bereits existiert
            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Name == roomDto.Name);

            if (existingRoom != null)
            {
                return Conflict("Ein Raum mit diesem Namen existiert bereits.");
            }

            var room = new Room
            {
                Name = roomDto.Name
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
        }

        // GET: api/room/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // GET: api/room/all
        [HttpGet("all")]
        public async Task<IActionResult> GetRooms()
        {
            // todo: ausprogrammieren
            var rooms = await _context.Rooms.ToListAsync();
            return Ok(rooms);
        }

        // post api/room/{id}/add-user
        [HttpPost("{roomId}/add-user")]
        public async Task<IActionResult> AddUserToRoom(int roomId, [FromBody] AddUserToRoomDto addUserDto)
        {
            var result = await _roomService.AddUserToRoom(roomId, addUserDto.UserId);
            if (!result)
            {
                return NotFound("Benutzer konnte nicht validiert werden oder Raum wurde nicht gefunden.");
            }

            return Ok("Benutzer wurde dem Raum hinzugefügt.");
        }
    }
}