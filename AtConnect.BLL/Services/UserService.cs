using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.Enum;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDTO<List<UserListItemDto>>> GetUsersAsync(int currentUserId, int page, int pageSize)
        {
            if (page < 1) return new (false, "Invalid page number", null);
            if (pageSize < 1) return new(false, "Invalid page size", null);
            var UsersPage= await _unitOfWork.Users.GetUsersAsync(currentUserId, page, pageSize);

            return new ResultDTO<List<UserListItemDto>>(true, "Users retrieved successfully", UsersPage);
        }

       
    }
}
