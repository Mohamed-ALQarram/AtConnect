using AtConnect.BLL.DTOs;
using AtConnect.Core.Enum;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Interfaces
{
    public interface IChatService
    {
        public Task<ResultDTO<PagedResultDto<UserChatDTO>>> GetUserChatsAsync(int userId, int page, int pageSize);
        public  Task<ResultDTO<PagedResultDto<Message>>> GetChatMessagesAsync(int chatId, int page = 1, int pageSize = 50);
        public Task<ResultDTO<PagedResultDto<ChatRequestDTO>>> getPendingChatRequestsAsync(int receiverId, int page = 1, int pageSize = 10);
        public Task SaveChatMessage(Message message);
        public Task<bool> IsChatParticipantAsync(int chatId, int userId);
        public Task<bool> MarkChatMessagesAsReadAsync(int chatId, int ReaderId);
        public Task<int?> GetOtherParticipantIdAsync(int chatId, int userId);
        public Task<ResultDTO<UserChatDTO>> GetChatByIdAsync(int chatId, int userId);
    }
}
