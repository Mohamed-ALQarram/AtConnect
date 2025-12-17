using AtConnect.Core.SharedDTOs;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace AtConnect.DAL.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly AppDbContext appDbContext;

        public NotificationRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<PagedResultDto<NotificationDTO>> GetByUserPagedAsync(int userId, int page, int pageSize)
        {
            if (page < 1) page = 1;

            var query = appDbContext.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .Select(n => new NotificationDTO
                {

                    UserId = n.UserId,
                    ChatId = n.ChatId,
                    RequestId = n.ChatRequestId,
                    Content = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                    notificationType = n.Type,
                    UserFullName = n.User.FirstName + " " + n.User.LastName,
                    AvatarUrl = n.User.ImageURL
                })
                .OrderByDescending(n => n.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<NotificationDTO>(items, totalCount, page, pageSize);
        }

        public async Task<IEnumerable<NotificationDTO>> GetUnreadNotificationsAsync(int userId)
        {
            return await appDbContext.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDTO
                {
                    
                    UserId = n.UserId,
                    ChatId = n.ChatId,
                    RequestId = n.ChatRequestId,
                    Content = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                    notificationType = n.Type,  
                    UserFullName = n.User.FirstName+ " " +n.User.LastName,
                    AvatarUrl = n.User.ImageURL
                })
                .ToListAsync();
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
          return await appDbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync<Notification>(n => n.SetProperty(p => p.IsRead, true));

        }
    }
}
