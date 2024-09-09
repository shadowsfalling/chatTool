using System.Security.Cryptography;
using System.Text;

namespace UserService.Services
{
    public class UserService
    {
        private readonly UserDbContext _userDbContext;

        public UserService(UserDbContext userDbContext) {
            _userDbContext = userDbContext;
        }

        public User Authenticate(string username, string password)
        {
            var user = _userDbContext.Users.SingleOrDefault(u => u.Username == username);
            if (user == null || user.Password != HashPassword(password))
            {
                return null;  // Authentication failed
            }
            return user;  // Authentication successful
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}