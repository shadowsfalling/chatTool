using Microsoft.EntityFrameworkCore;

namespace ChatService.Services
{
    public class SeederService
    {
        private readonly ChatDbContext _context;

        public SeederService(ChatDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (!_context.Messages.Any())
            {
                _context.Messages.Add(new Message
                {
                    Content =  "Hi, how are you?",
                    RoomId = 1,
                    Timestamp = DateTime.Now,
                    UserId = "testuser"
                });

                _context.Messages.Add(new Message
                {
                    Content =  "I am fine, how about you?",
                    RoomId = 1,
                    Timestamp = DateTime.Now,
                    UserId = "otheruser"
                });

                _context.Messages.Add(new Message
                {
                    Content =  "I am fine, too! Thank you.",
                    RoomId = 1,
                    Timestamp = DateTime.Now,
                    UserId = "testuser"
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