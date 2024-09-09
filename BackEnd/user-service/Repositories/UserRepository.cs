using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace UserService.Repositories
{
    public class UserRepository
    {
        private readonly UserDbContext _userDbContext;

        public UserRepository(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public async Task<User> AddAsync(User user)
        {
            _userDbContext.Users.Add(user);
            await _userDbContext.SaveChangesAsync();  // Asynchron speichern
            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username) {
            return await _userDbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}