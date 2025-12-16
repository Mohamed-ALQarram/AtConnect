using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;

namespace AtConnect.Core.Interfaces
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<bool> IsParticipantAsync(int chatId, int userId);
        Task<PagedResultDto<UserChatDTO>> GetUserChatsAsync(int userId, int page, int pageSize);
        Task<int?> GetOtherParticipantIdAsync(int chatId, int userId);
        Task<UserChatDTO?> GetChatByIdAsync(int chatId, int userId);
    }

}
