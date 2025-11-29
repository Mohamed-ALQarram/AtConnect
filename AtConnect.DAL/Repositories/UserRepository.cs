using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace AtConnect.DAL.Repositories
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        private readonly AppDbContext appDbContext;

        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<bool> CheckEmailAsync(string email)
        {
            return await appDbContext.AppUsers.AnyAsync(u=>u.Email == email &&u.isEmailVerified==true);
        }

        public async Task<bool> CheckUserNameAsync(string username)
        {
            return await appDbContext.AppUsers.AnyAsync(u => u.UserName == username && u.isEmailVerified == true);
        }

        public async Task<AppUser?> GetByUserNameOrEmailAsync(string UserNameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(UserNameOrEmail)) throw new ArgumentNullException("invalid UserName or Email");
            return await appDbContext.AppUsers.FirstOrDefaultAsync(x=>x.UserName ==  UserNameOrEmail || x.Email == UserNameOrEmail);
        }
    }
}
