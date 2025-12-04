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

        public async Task<List<Notification>> GetByUserPagedAsync(int userId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            return await appDbContext.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId)
        {
            return await appDbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
