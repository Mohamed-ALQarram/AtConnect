using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("AllUsers")]
        public async Task<ActionResult<ResultDTO<List<UserListItemDto>>>> GetUsers([FromQuery] PaginationRequest request)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int userId);
            var response = await _userService.GetUsersAsync(userId, request.Page, request.PageSize);
            if (!response.Success)
                return BadRequest(response);
            return response;
        }

    }

}
