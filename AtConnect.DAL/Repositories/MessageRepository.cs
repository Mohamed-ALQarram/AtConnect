using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;

namespace AtConnect.DAL.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly AppDbContext appDbContext;

        public MessageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public Task<IEnumerable<Message>> GetMessagesForChatAsync(int chatId, int count = 50)
        {
            throw new NotImplementedException();
        }
    }
}
