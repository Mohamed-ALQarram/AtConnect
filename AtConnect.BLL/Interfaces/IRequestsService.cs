using AtConnect.BLL.DTOs;
using AtConnect.Core.SharedDTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Interfaces
{
    public interface IRequestService
    {
        Task<ResultDTO<bool>> SendRequestAsync(int senderId, int toUserId);
    }
}
