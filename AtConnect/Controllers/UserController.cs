using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.SharedDTOs;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("AllUsers")]
        public async Task<ActionResult<ResultDTO<PagedResultDto<UserListItemDto>>>> GetUsers([FromQuery] PaginationRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ResultDTO<PagedResultDto<UserListItemDto>>(false, "Invalid or missing user ID in token"));

            var response = await _userService.GetUsersAsync(userId.Value, request.Page, request.PageSize);
            if (!response.Success)
                return BadRequest(response);
            return response;
        }
        [HttpGet("UserProfile")]
        public async Task<ActionResult<ResultDTO<UserListItemDto>>> GetUserProfile(int targetUserId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
                return Unauthorized(new ResultDTO<List<UserListItemDto>>(false, "Invalid or missing user ID in token"));

            var response = await _userService.GetUserProfileByIdAsync((int) currentUserId, targetUserId);
            if (!response.Success)
                return BadRequest(response);
            return response;
        }
        [HttpPut("EditProfile")]
        public async Task<ActionResult<ResultDTO<object>>> UpdateUserProfile(UpdateProfileRequest updateProfile)
        {
            var userId= GetCurrentUserId();
            if (userId == null) return Unauthorized(new ResultDTO<object>(false, "Invalid or missing user ID in token", null));
            var response = await _userService.UpdateUserProfileAsync((int)userId, updateProfile.FirstName, updateProfile.LastName, updateProfile.ProfileImageUrl, updateProfile.Bio, updateProfile.About);
                if(!response.Success)
                    return BadRequest(response);
            return response;

        }
    }

}
