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
        
        public async Task<PagedResultDto<UserChatDTO>> GetUserChatsAsync(int userId, int page, int pageSize)
        {
            var query = appDbContext.Chats
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Include(c => c.Messages)
                .Include(c=>c.User1)
                .Include(c => c.User2)
                .Select(c => new
                {
                    Chat = c,
                    OtherUser = c.User1Id == userId ? c.User2 : c.User1,
                    MostRecentMessage = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault(),
                    UnreadMessageCount = c.Messages
                        .Count(m => m.Status != MessageStatus.Seen && m.SenderId != userId),
                    // Use this for ordering - null-safe
                    MostRecentMessageDate = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => (DateTime?)m.SentAt)
                        .FirstOrDefault()
                })
                .OrderByDescending(c => c.MostRecentMessageDate ?? DateTime.MinValue);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new UserChatDTO(
                    c.OtherUser.Id,
                    c.OtherUser.ImageURL ?? "",
                    c.OtherUser.IsActive,
                    c.MostRecentMessage != null ? c.MostRecentMessage.Content : string.Empty,
                    c.MostRecentMessage != null ? c.MostRecentMessage.SentAt : DateTime.MinValue,
                    c.UnreadMessageCount))
                .ToListAsync();

            return new PagedResultDto<UserChatDTO>(items, totalCount, page, pageSize);
        }

        public async Task<bool> IsParticipantAsync(int chatId, int userId)
        {
            return await appDbContext.Chats.Where(x=>x.Id == chatId).AnyAsync(x=>x.User1Id == userId || x.User2Id== userId);
        }

        public async Task<int?> GetOtherParticipantIdAsync(int chatId, int userId)
        {
            if (chatId < 1 || userId <1) throw new ArgumentException("Invalid Arguments");
            var chat = await appDbContext.Chats
                .Where(c => c.Id == chatId && (c.User1Id == userId || c.User2Id == userId))
                .Select(c => new { c.User1Id, c.User2Id })
                .FirstOrDefaultAsync();

            if (chat == null) return null;

            return chat.User1Id == userId ? chat.User2Id : chat.User1Id;
        }

        public async Task<UserChatDTO?> GetChatByIdAsync(int chatId, int userId)
        {
            if (chatId < 1 || userId < 1) return null;

            var result = await appDbContext.Chats
                .Where(c => c.Id == chatId && (c.User1Id == userId || c.User2Id == userId))
                .Include(c => c.Messages)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Select(c => new
                {
                    OtherUser = c.User1Id == userId ? c.User2 : c.User1,
                    MostRecentMessage = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault(),
                    UnreadMessageCount = c.Messages
                        .Count(m => m.Status != MessageStatus.Seen && m.SenderId != userId)
                })
                .FirstOrDefaultAsync();

            if (result == null) return null;

            return new UserChatDTO(
                result.OtherUser.Id,
                result.OtherUser.ImageURL ?? "",
                result.OtherUser.IsActive,
                result.MostRecentMessage != null ? result.MostRecentMessage.Content : string.Empty,
                result.MostRecentMessage != null ? result.MostRecentMessage.SentAt : DateTime.MinValue,
                result.UnreadMessageCount);
        }
    }
}
