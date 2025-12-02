using AtConnect.Core.Enum;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
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

        public async Task<IEnumerable<UserChatDTO>> GetUserChatsAsync(int userId, int page, int pageSize)
        {
             return await appDbContext.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Skip((page-1) * pageSize)
                .Take(pageSize)
                    .Select(c => new 
                    {
                        OtherUser = c.User1Id == userId ? c.User2 : c.User1,
                        MostRecentMessage = c.Messages
                             .OrderByDescending(m => m.SentAt)
                             .FirstOrDefault() ?? new Message(0,0,""),
                        UnreadMessageCount = c.Messages
                             .Count(m => m.Status != MessageStatus.Seen && m.SenderId != userId)
                    })
                    .OrderByDescending(c => c.MostRecentMessage.SentAt)
                    .Select(c => new UserChatDTO(
                        c.OtherUser.Id,
                        c.OtherUser.ImageURL,
                        c.OtherUser.IsActive,
                        c.MostRecentMessage.Content, 
                        c.MostRecentMessage.SentAt, 
                        c.UnreadMessageCount))
                .ToListAsync();
        }
    }
}
