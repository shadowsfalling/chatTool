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

        public RoomController(RoomDbContext context)
        {
            _context = context;
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
    }
}