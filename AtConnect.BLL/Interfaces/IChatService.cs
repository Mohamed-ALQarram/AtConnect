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
        public Task<ResultDTO<List<UserChatDTO>>> GetUserChatsAsync(int userId, int page, int pageSize);
        public  Task<ResultDTO<List<Message>>> GetChatMessagesAsync(int chatId, int page = 1, int pageSize = 50);
        public Task<ResultDTO<object>> ChangeRequestStatusAsync(int requestId, RequestStatus status);
        public Task<ResultDTO<List<ChatRequestDTO>>> getPendingChatRequestsAsync(int receiverId, int page = 1, int pageSize = 10);
    }
}
