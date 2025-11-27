using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;

namespace AtConnect.DAL.Repositories
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        private readonly AppDbContext appDbContext;

        public ChatRepository(AppDbContext appDbContext):base(appDbContext) 
        {
            this.appDbContext = appDbContext;
        }

        public Task<Chat?> GetChatBetweenAsync(int userAId, int userBId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Chat>> GetUserChatsAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
