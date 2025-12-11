using AtConnect.Core.SharedDTOs;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace AtConnect.DAL.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly AppDbContext appDbContext;

        public MessageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task AddRangeMessagesAsync(List<Message> messages)
        {
            if (messages != null && messages.Count != 0)
                await appDbContext.Messages.AddRangeAsync(messages);
        }
        public async Task<PagedResultDto<Message>> GetChatMessagesAsync(int chatId, int page = 1, int pageSize = 50)
        {
            var query = appDbContext.Messages
                .Where(msg => msg.ChatId == chatId)
                .OrderByDescending(msg => msg.SentAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<Message>(items, totalCount, page, pageSize);
        }

    }
}
