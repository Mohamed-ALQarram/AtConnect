using AtConnect.Core.Enum;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;

namespace AtConnect.Core.Interfaces
{
    public interface IChatRequestRepository : IGenericRepository<ChatRequest>
    {
        public Task<List<ChatRequestDTO>> GetPendingRequestAsync(int receiverId, int page, int pageSize);

        public Task<bool> ChangeRequestStatusAsync(int RequestId, RequestStatus newStatus);
    }

}
