using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
        Task<bool> CheckEmailAsync(string email);
        Task<bool> CheckUserNameAsync(string username);
        Task<AppUser?> GetByUserNameOrEmailAsync(string UserNameOrEmail);
        
    }

}
