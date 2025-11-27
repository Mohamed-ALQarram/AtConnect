using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IChatRequestRepository : IGenericRepository<ChatRequest>
    {
        Task<ChatRequest?> GetExistingRequestAsync(int senderId, int receiverId);
        Task<IEnumerable<ChatRequest>> GetPendingRequestsAsync(int userId);
    }

}
