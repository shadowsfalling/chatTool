using Microsoft.EntityFrameworkCore;

namespace RoomService.Services
{
    public class SeederService
    {
        private readonly RoomDbContext _context;

        public SeederService(RoomDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (!_context.Rooms.Any())
            {
                _context.Rooms.Add(new Room
                {
                    IsPrivate = false,
                    Name = "my Room"
                });

                _context.Rooms.Add(new Room
                {
                    IsPrivate = false,
                    Name = "testroom"
                });

                _context.RoomUsers.Add(new RoomUser
                {
                    RoomId = 1,
                    UserId = 1
                });

                _context.RoomUsers.Add(new RoomUser
                {
                    RoomId = 1,
                    UserId = 2
                });

                _context.RoomUsers.Add(new RoomUser
                {
                    RoomId = 2,
                    UserId = 1
                });

                _context.SaveChanges();
            }
        }

        // Methode zum Zurücksetzen der Datenbank
        public void ResetDatabase()
        {
            _context.Database.EnsureDeleted();
            _context.Database.Migrate();
        }
    }
}