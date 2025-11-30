using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace AtConnect.DAL.Repositories
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        private readonly AppDbContext appDbContext;

        public ChatRepository(AppDbContext appDbContext):base(appDbContext) 
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Chat?> GetChatBetweenAsync(int userAId, int userBId)
        {
            return await appDbContext.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => (c.User1Id == userAId && c.User2Id == userBId) || 
                                          (c.User1Id == userBId && c.User2Id == userAId));
        }

        public async Task<IEnumerable<Chat>> GetUserChatsAsync(int userId)
        {
            return await appDbContext.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync();
        }
    }
}
