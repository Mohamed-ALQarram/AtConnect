using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesForChatAsync(int chatId, int count = 50);
    }

}
