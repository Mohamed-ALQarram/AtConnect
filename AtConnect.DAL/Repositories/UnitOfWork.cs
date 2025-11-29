using AtConnect.Core.Interfaces;
using AtConnect.DAL.Data;

namespace AtConnect.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext appDbContext;

        public IUserRepository Users {  get; private set; }
        public IChatRepository Chats { get; private set; }
        public IMessageRepository Messages { get; private set; }
        public IChatRequestRepository ChatRequests { get; private set; }
        public INotificationRepository Notifications { get; private set; }
        public IDeviceTokenRepository DeviceTokens { get; private set; }

        public UnitOfWork(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
            Users = new UserRepository(appDbContext);
            Chats = new ChatRepository(appDbContext);
            ChatRequests = new ChatRequestRepository(appDbContext);
            Messages = new MessageRepository(appDbContext);
            Notifications = new NotificationRepository(appDbContext);
            DeviceTokens = new DeviceTokenRepository(appDbContext);
        }
        public void Dispose()
        {
            appDbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await appDbContext.SaveChangesAsync();
        }
    }
}
