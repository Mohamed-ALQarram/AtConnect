using AtConnect.Core.SharedDTOs;
using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        public Task AddRangeMessagesAsync(List<Message> messages);
        public Task<PagedResultDto<Message>> GetChatMessagesAsync(int chatId, int page = 1, int pageSize = 50);
        public Task<bool> MarkMessagesAsReadAsync(int chatId, int ReaderId);

    }

}
