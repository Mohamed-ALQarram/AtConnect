using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;

namespace AtConnect.DAL.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly AppDbContext appDbContext;

        public NotificationRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
