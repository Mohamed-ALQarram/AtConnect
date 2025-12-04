using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
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

        public async Task<List<UserListItemDto>> GetUsersAsync(int currentUserId, int page, int pageSize)
        {
            var query =
                from user in GetAll()
                where user.Id != currentUserId
                select new UserListItemDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    ProfilePhotoUrl = user.ImageURL?? "",
                    isActive = user.IsActive,
                    //left join logic:
                    ChatRequest = user.ChatRequests!
                        .FirstOrDefault(cr =>
                               (cr.SenderId == currentUserId && cr.ReceiverId == user.Id)
                            || (cr.ReceiverId == currentUserId && cr.SenderId == user.Id))
                };

            // 3) Apply pagination in memory
            var paged = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return await paged;
        }
    }
}
