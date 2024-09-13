using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Repositories
{
    public class MessageRepository
    {
        private readonly ChatDbContext _chatDbContext;

        public MessageRepository(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }

        public async Task<Message> AddAsync(Message message)
        {
            _chatDbContext.Messages.Add(message);
            await _chatDbContext.SaveChangesAsync();  // Asynchron speichern
            return message;
        }

        public async Task<IEnumerable<Message>> GetMessagesByRoomIdAsync(int roomId)
        {
            return _chatDbContext.Messages.Where(m => m.RoomId == roomId);
        }
    }
}