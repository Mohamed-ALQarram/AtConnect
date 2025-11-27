using AtConnect.Core.Models;

namespace AtConnect.Core.Interfaces
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetByUserNameAsync(string username);
    }

}
