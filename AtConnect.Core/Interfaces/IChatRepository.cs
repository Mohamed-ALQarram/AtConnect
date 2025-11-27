using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<Chat?> GetChatBetweenAsync(int userAId, int userBId);
        Task<IEnumerable<Chat>> GetUserChatsAsync(int userId);
    }

}
