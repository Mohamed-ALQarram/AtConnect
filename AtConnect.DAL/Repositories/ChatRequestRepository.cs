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

        public async Task<PagedResultDto<ChatRequestDTO>> GetPendingRequestAsync(int receiverId, int page, int pageSize)
        {
            var query = appDbContext.ChatRequests
                                    .Include(r => r.Sender)
                                    .Where(x => x.ReceiverId == receiverId &&
                                               x.Status == RequestStatus.Pending);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ChatRequestDTO
                (x.Id, x.SenderId, $"{x.Sender.FirstName}  {x.Sender.LastName}", x.Sender.ImageURL ?? "", 0))
                .ToListAsync();

            return new PagedResultDto<ChatRequestDTO>(items, totalCount, page, pageSize);
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
