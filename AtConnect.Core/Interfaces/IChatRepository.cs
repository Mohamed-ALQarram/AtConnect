using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;

namespace AtConnect.Core.Interfaces
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<Chat?> GetChatBetweenAsync(int userAId, int userBId);
        Task<IEnumerable<UserChatDTO>> GetUserChatsAsync(int userId, int page, int pageSize);

    }

}
