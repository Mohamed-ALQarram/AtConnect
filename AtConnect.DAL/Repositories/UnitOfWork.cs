using AtConnect.Core.Interfaces;

namespace AtConnect.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository Users => throw new NotImplementedException();

        public IChatRepository Chats => throw new NotImplementedException();

        public IMessageRepository Messages => throw new NotImplementedException();

        public IChatRequestRepository ChatRequests => throw new NotImplementedException();

        public INotificationRepository Notifications => throw new NotImplementedException();

        public IDeviceTokenRepository DeviceTokens => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
