using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;

namespace AtConnect.DAL.Repositories
{
    public class ChatRequestRepository: GenericRepository<ChatRequest>, IChatRequestRepository
    {
        private readonly AppDbContext appDbContext;

        public ChatRequestRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public Task<ChatRequest?> GetExistingRequestAsync(int senderId, int receiverId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ChatRequest>> GetPendingRequestsAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
