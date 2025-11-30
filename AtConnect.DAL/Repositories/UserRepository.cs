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
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("invalid UserName or Email");

            return await appDbContext.AppUsers.AnyAsync(u=>u.Email == email &&u.isEmailVerified==true);
        }
        public async Task<bool> CheckUserNameAsync(string UserName)
        {
            if (string.IsNullOrWhiteSpace(UserName))
                throw new ArgumentNullException("invalid UserName or Email");

            return await appDbContext.AppUsers.AnyAsync(u => u.UserName == UserName);
        }
        public async Task<bool> CheckUnVerifiedEmailAsync(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentNullException("invalid UserName or Email");

            return await appDbContext.AppUsers.AnyAsync(u => u.Email == Email && u.isEmailVerified == false);
        }

        public async Task<AppUser?> GetByUserNameOrEmailAsync(string UserNameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(UserNameOrEmail)) throw new ArgumentNullException("invalid UserName or Email");
            return await appDbContext.AppUsers.FirstOrDefaultAsync(x=>x.UserName ==  UserNameOrEmail || x.Email == UserNameOrEmail);
        }
    }
}
