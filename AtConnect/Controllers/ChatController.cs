using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.BLL.Services;
using AtConnect.Core.Enum;
using AtConnect.Core.Models;
using AtConnect.Core.SharedDTOs;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Claims;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IRequestService _requestService;

        public ChatController(IChatService chatService, IRequestService requestService)
        {
            _chatService = chatService;
            _requestService = requestService;
        }
        [HttpGet("UserChats")]
        public async Task<ActionResult<ResultDTO<List<UserChatDTO>>>> GetUserChats([FromQuery]  PaginationRequest request)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int userId);
            var response = await _chatService.GetUserChatsAsync(userId, request.Page, request.PageSize);
                if(!response.Success)
                return Unauthorized(response);
            return response;

        }

        [HttpGet("ChatRequests")]
        public async Task<ActionResult<ResultDTO<List<ChatRequestDTO>>>> GetChatRequests([FromQuery] PaginationRequest request)
        {
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x=>x.Type ==ClaimTypes.NameIdentifier)?.Value, out int userId);
            var response = await _chatService.getPendingChatRequestsAsync(userId, request.Page, request.PageSize);
            if (!response.Success)
                return Unauthorized(response);
            return response;
        }

        [HttpPost("SendChatRequest")]
        public async Task<ActionResult<ResultDTO<bool>>> SendRequest([FromBody] SendRequestDto dto)
        {
            if (dto == null) return BadRequest(new ResultDTO<bool>(false, "Invalid body."));

            int.TryParse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int senderId);

            if (dto.ToUserId <= 0)
                return BadRequest(new ResultDTO<bool>(false, "Couldn't find the user ID in the bearer token."));

            var response = await _requestService.SendRequestAsync(senderId, dto.ToUserId);
            if (!response.Success)
                return BadRequest(response);

            return response;
        }


        [HttpPost("AcceptRequest")]
        public async Task<ActionResult<ResultDTO<object>>> ChangeChatRequestStatus(int RequestId, bool isAccepted)
        {
            RequestStatus status;
            if (isAccepted)
                status = RequestStatus.Accepted;
            else
                status = RequestStatus.Rejected;
            var response=  await _chatService.ChangeRequestStatusAsync(RequestId, status);
            if(!response.Success)
                return BadRequest(response);
            return response;

        }
        [HttpGet("ChatMessages")]
        public async Task<ActionResult<ResultDTO<List<Message>>>> getChatMessages(int chatId, PaginationRequest request)
        {
            var response = await _chatService.GetChatMessagesAsync(chatId, request.Page, request.PageSize);
            if(!response.Success) return Unauthorized(response);
            return response;
        }
    }
}
