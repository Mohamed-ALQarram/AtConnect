using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.Enum;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<ResultDTO<PagedResultDto<UserChatDTO>>>  GetUserChatsAsync(int userId, int page=1, int pageSize=10)
        {
            if (userId <= 0 || page<0 || pageSize<0)
                return  new ResultDTO<PagedResultDto<UserChatDTO>>(false, "Invalid Arguments", null);

            var Chats = await _unitOfWork.Chats.GetUserChatsAsync(userId, page, pageSize);
            return   new ResultDTO<PagedResultDto<UserChatDTO>>(true,null, Chats);
        }
        
        public async Task<ResultDTO<PagedResultDto<Message>>> GetChatMessagesAsync(int chatId, int page=1, int pageSize=50)
        {
            var Messages= await _unitOfWork.Messages.GetChatMessagesAsync(chatId, page, pageSize);
            if (Messages == null) return new ResultDTO<PagedResultDto<Message>>(true, null, null);
            return new (true, null, Messages);
        }

        public async Task<ResultDTO<PagedResultDto<ChatRequestDTO>>> getPendingChatRequestsAsync(int receiverId, int page=1, int pageSize=10)
        {
            if (receiverId <= 0 || page <= 0 || pageSize <= 0) return new(false, "Invalid arguments", null);
            var Requests=  await _unitOfWork.ChatRequests.GetPendingRequestAsync(receiverId, page, pageSize);
            return new ResultDTO<PagedResultDto<ChatRequestDTO>>(true, null, Requests);
        }

        public async Task<ResultDTO<object>> ChangeRequestStatusAsync(int requestId, RequestStatus status)
        {
            var success= await _unitOfWork.ChatRequests.ChangeRequestStatusAsync(requestId, status);
            if (!success)
                return new ResultDTO<object>(success, "Invalid RequestId", null);
            return new ResultDTO<object>(true, "Request status has been recorded successfully.", null);
        }

        public async Task SaveChatMessage(Message message)
        {
            await _unitOfWork.Messages.AddAsync(message); 
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsChatParticipantAsync(int chatId, int userId)
        {
            return await _unitOfWork.Chats.IsParticipantAsync(chatId, userId);
        }

        public async Task<bool> MarkChatMessagesAsReadAsync(int chatId, int ReaderId)
        {
            if (chatId < 1 || ReaderId < 1) return  false;
            return await _unitOfWork.Messages.MarkMessagesAsReadAsync(chatId, ReaderId);
        }

        public async Task<int?> GetOtherParticipantIdAsync(int chatId, int userId)
        {
            if(chatId <1) return null;
            return await _unitOfWork.Chats.GetOtherParticipantIdAsync(chatId, userId);
        }
    }
}
