using Microsoft.EntityFrameworkCore;

namespace UserService.Services
{
    public class SeederService
    {
        private readonly UserDbContext _context;

        public SeederService(UserDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (!_context.Users.Any())
            {
                _context.Users.Add(new User
                {
                    Username = "testuser",
                    Email = "testuser@example.com",
                    Password = UserService.HashPassword("password123")
                });

                _context.Users.Add(new User
                {
                    Username = "otheruser",
                    Email = "otheruser@example.com",
                    Password = UserService.HashPassword("password123")
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