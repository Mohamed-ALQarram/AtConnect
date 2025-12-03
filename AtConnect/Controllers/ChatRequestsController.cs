using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Services;
using AtConnect.Core.SharedDTOs;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatRequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;
        public ChatRequestsController(IRequestService requestService, IUserService userService)
        {
            _requestService = requestService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDTO<List<UserListItemDto>>>> GetUsers([FromQuery] PaginationRequest request)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int userId);
            var response = await _userService.GetUsersAsync(userId, request.Page, request.PageSize);
            if (!response.Success)
                return Unauthorized(response);
            return response;
        }

        [HttpPost("send")]
        public async Task<ActionResult<ResultDTO<bool>>> SendRequest([FromBody] SendRequestDto dto)
        {
            if (dto == null) return BadRequest(new ResultDTO<bool>(false, "Invalid body."));

            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int senderId);

            if (dto.ToUserId <= 0)
                return BadRequest(new ResultDTO<bool>(false, "toUserId is required and must be > 0."));

            var response = await _requestService.SendRequestAsync(senderId, dto.ToUserId);
            if (!response.Success)
                return BadRequest(response);

            return response;
        }
    }
}

