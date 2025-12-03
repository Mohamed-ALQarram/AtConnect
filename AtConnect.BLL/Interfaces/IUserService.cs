using AtConnect.BLL.DTOs;
using AtConnect.Core.Enum;
using AtConnect.Core.SharedDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Interfaces
{
    public interface IUserService
    {
        Task<ResultDTO<List<UserListItemDto>>> GetUsersAsync(int currentUserId, int page, int pageSize);
        
    }
}
