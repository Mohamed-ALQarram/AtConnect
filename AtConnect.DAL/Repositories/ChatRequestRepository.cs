using AtConnect.Core.Enum;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using AtConnect.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace AtConnect.DAL.Repositories
{
    public class ChatRequestRepository: GenericRepository<ChatRequest>, IChatRequestRepository
    {
        private readonly AppDbContext appDbContext;

        public ChatRequestRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<ChatRequestDTO>> GetPendingRequestAsync(int receiverId, int page=1, int pageSize=10)
        {
            return await appDbContext.ChatRequests
                .Where(x=> x.ReceiverId == receiverId &&
                    x.Status == RequestStatus.Pending)
                .Skip((page -1 )*pageSize)
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Take(pageSize)
                .Select(x => new ChatRequestDTO
                ( x.Id, x.SenderId, $"{x.Sender.FirstName}  {x.Sender.LastName}", x.Sender.ImageURL??"",0)).ToListAsync();
        }
        
        public async Task<bool> ChangeRequestStatusAsync(int RequestId, RequestStatus newStatus)
        {
            var result =await appDbContext.ChatRequests
                .Where(x=>x.Id == RequestId)
                .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.Status, newStatus));
            if (result > 0)
                return true;
            return false;
        }
    }
}
