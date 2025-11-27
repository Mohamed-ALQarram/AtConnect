using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;

namespace AtConnect.DAL.Repositories
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        private readonly AppDbContext appDbContext;

        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public Task<AppUser?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser?> GetByUserNameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}
