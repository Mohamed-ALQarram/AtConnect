using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
        Task<bool> CheckEmailAsync(string email);
        Task<bool> CheckUserNameAsync(string UserName);
        Task<bool> CheckUnVerifiedEmailAsync(string Email);
        Task<AppUser?> GetByUserNameOrEmailAsync(string UserNameOrEmail);
        
    }

}
