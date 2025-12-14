using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using AtConnect.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public async Task<PagedResultDto<UserListItemDto>> GetUsersAsync(int currentUserId, int page, int pageSize)
        {
            var query =
                from user in appDbContext.AppUsers
                where user.Id != currentUserId && user.isEmailVerified == true
                select new UserListItemDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    ProfilePhotoUrl = user.ImageURL ?? "",
                    isActive = user.IsActive,
                    AboutUser = user.AboutUser ?? "",
                    UserName = user.UserName,
                    // Proper left join with ChatRequests
                    ChatRequest = appDbContext.ChatRequests
                        .Where(cr =>
                               ((cr.SenderId == currentUserId && cr.ReceiverId == user.Id)
                            || (cr.ReceiverId == currentUserId && cr.SenderId == user.Id)))
                        .OrderByDescending(cr => cr.CreatedAt)
                        .FirstOrDefault()
                };

            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<UserListItemDto>(items, totalCount, page, pageSize);
        }
    
        public async Task<UserListItemDto?> GetUserProfileAsync(int currentUserId, int targetUserId)
        {
            var query =
                from user in appDbContext.AppUsers
                where user.Id == targetUserId
                select new UserListItemDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    ProfilePhotoUrl = user.ImageURL ?? "",
                    isActive = user.IsActive,
                    AboutUser = user.AboutUser ?? "",
                    UserName = user.UserName,
                    // Proper left join with ChatRequests
                    ChatRequest = appDbContext.ChatRequests
                        .Where(cr =>
                               ((cr.SenderId == currentUserId && cr.ReceiverId == targetUserId)
                            || (cr.ReceiverId == currentUserId && cr.SenderId == targetUserId)))
                        .OrderByDescending(cr => cr.CreatedAt)
                        .FirstOrDefault()
                };
            
            return await query.FirstOrDefaultAsync();
        }
    }
}
