using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        public Task AddRangeMessagesAsync(List<Message> messages);
        public Task<IEnumerable<Message>> GetChatMessagesAsync(int chatId, int page = 1, int pageSize = 50);

    }

}
