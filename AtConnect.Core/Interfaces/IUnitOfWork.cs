namespace AtConnect.Core.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        IUserRepository Users { get; }
        IChatRepository Chats { get; }
        IMessageRepository Messages { get; }
        IChatRequestRepository ChatRequests { get; }
        INotificationRepository Notifications { get; }
        IDeviceTokenRepository DeviceTokens { get; }

        Task<int> SaveChangesAsync();
    }


}
