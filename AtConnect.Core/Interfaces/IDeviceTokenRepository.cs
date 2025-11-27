using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IDeviceTokenRepository : IGenericRepository<DeviceToken>
    {
        Task<IEnumerable<DeviceToken>> GetActiveTokensAsync(int userId);
    }


}
