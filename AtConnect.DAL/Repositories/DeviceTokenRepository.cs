using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;

namespace AtConnect.DAL.Repositories
{
    public class DeviceTokenRepository : GenericRepository<DeviceToken>, IDeviceTokenRepository
    {
        private readonly AppDbContext appDbContext;

        public DeviceTokenRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public Task<IEnumerable<DeviceToken>> GetActiveTokensAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
