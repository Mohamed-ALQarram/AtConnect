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

        public async Task<ResultDTO<PagedResultDto<UserListItemDto>>> GetUsersAsync(int currentUserId, int page, int pageSize)
        {
            if (page < 1) return new (false, "Invalid page number", null);
            if (pageSize < 1) return new(false, "Invalid page size", null);
            var UsersPage= await _unitOfWork.Users.GetUsersAsync(currentUserId, page, pageSize);

            return new ResultDTO<PagedResultDto<UserListItemDto>>(true, "Users retrieved successfully", UsersPage);
        }
        public async Task<ResultDTO<UserListItemDto>> GetUserProfileByIdAsync(int currentUserId, int targetUserId)
        {
            var user= await _unitOfWork.Users.GetUserProfileAsync(currentUserId, targetUserId);
            if (user == null) return new(false, "Invalid user ID", null);

            return new ResultDTO<UserListItemDto>(true, "Users retrieved successfully", user);
        }

        public async Task<ResultDTO<object>> UpdateUserProfileAsync(int userId, string? FirstName, string? LastName, string? ProfileImageUrl, string? Bio, string? About)
        {
            var user=  await _unitOfWork.Users.GetByKeysAsync(userId);
            if (user == null) return new(false, "Invalid UserId", null);

            if(!string.IsNullOrWhiteSpace(FirstName))
                user.ChangeFirstName(FirstName);

            if(!string.IsNullOrWhiteSpace(LastName))
                user.ChangeLastName(LastName);

            if(!string.IsNullOrWhiteSpace(ProfileImageUrl))
                user.ChangeImage(ProfileImageUrl);

            if(!string.IsNullOrWhiteSpace(Bio))
                user.ChangeBio(Bio);

            if (!string.IsNullOrWhiteSpace(About))
                user.ChangeAboutUser(About);
            await _unitOfWork.SaveChangesAsync();
            return new ResultDTO<object>(true, "Saved Successfully", null);
            
        }
    }
}
