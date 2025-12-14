using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.Enum;
using AtConnect.Core.Models;
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
    public class ChatController : ApiControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IRequestService _requestService;

        public ChatController(IChatService chatService, IRequestService requestService)
        {
            _chatService = chatService;
            _requestService = requestService;
        }
        [HttpGet("UserChats")]
        public async Task<ActionResult<ResultDTO<PagedResultDto<UserChatDTO>>>> GetUserChats([FromQuery]  PaginationRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ResultDTO<PagedResultDto<UserChatDTO>>(false, "Invalid or missing user ID in token"));

            var response = await _chatService.GetUserChatsAsync(userId.Value, request.Page, request.PageSize);
            if(!response.Success)
                return BadRequest(response);
            return response;
        }

        [HttpGet("ChatRequests")]
        public async Task<ActionResult<ResultDTO<PagedResultDto<ChatRequestDTO>>>> GetChatRequests([FromQuery] PaginationRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ResultDTO<PagedResultDto<ChatRequestDTO>>(false, "Invalid or missing user ID in token"));

            var response = await _chatService.getPendingChatRequestsAsync(userId.Value, request.Page, request.PageSize);
            if (!response.Success)
                return BadRequest(response);
            return response;
        }

        [HttpPost("SendChatRequest")]
        public async Task<ActionResult<ResultDTO<bool>>> SendRequest([FromBody] SendRequestDto dto)
        {
            if (dto == null) return BadRequest(new ResultDTO<bool>(false, "Invalid body."));

            var senderId = GetCurrentUserId();
            if (senderId == null)
                return Unauthorized(new ResultDTO<bool>(false, "Invalid or missing user ID in token"));

            if (dto.ToUserId <= 0)
                return BadRequest(new ResultDTO<bool>(false, "Invalid recipient user ID."));

            var response = await _requestService.SendRequestAsync(senderId.Value, dto.ToUserId);
            if (!response.Success)
                return BadRequest(response);

            return response;
        }


        [HttpPost("AcceptRequest")]
        public async Task<ActionResult<ResultDTO<object>>> ChangeChatRequestStatus([FromBody] ChangeRequestStatusDto dto)
        {
            var status = dto.IsAccepted ? RequestStatus.Accepted : RequestStatus.Rejected;
            var response = await _chatService.ChangeRequestStatusAsync(dto.RequestId, status);
            if(!response.Success)
                return BadRequest(response);
            return response;

        }
        [HttpGet("ChatMessages")]
        public async Task<ActionResult<ResultDTO<PagedResultDto<Message>>>> GetChatMessages([FromQuery] GetChatMessagesRequest request)
        {
            var response = await _chatService.GetChatMessagesAsync(request.ChatId, request.Page, request.PageSize);
            if(!response.Success) return BadRequest(response);
            return response;
        }
    }
}
