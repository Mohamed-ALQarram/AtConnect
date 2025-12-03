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
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo ;
        }
        public async Task<ResultDTO<List<UserListItemDto>>> GetUsersAsync(int currentUserId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            // 1) Retrieve all users (IGenericRepository commonly exposes GetAllAsync)
            var allUsers =  _userRepo.GetAll(); // <--- typical method on many generic repos

            // 2) Filter out the current user
            var others = allUsers
                .Where(u => u.Id != currentUserId)
                
                .ToList();

            // 3) Apply pagination in memory
            var paged = others
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserListItemDto
                {
                    Id = u.Id,
                    FullName = (u.FirstName + " " + u.LastName).Trim(),
                    ProfilePhotoUrl = u.ImageURL ?? string.Empty
                })
                .ToList();

            return new ResultDTO<List<UserListItemDto>>(true, "Users retrieved successfully", paged);
        }

       
    }
}
