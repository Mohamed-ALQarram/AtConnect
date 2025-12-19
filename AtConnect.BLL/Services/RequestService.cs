using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.SharedDTOs;
using AtConnect.Core.Enum;
using AtConnect.Core.Interfaces;
using AtConnect.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AtConnect.BLL.Services
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _uow;
        private readonly INotifier _notifier;

        public RequestService(IUnitOfWork uow, INotifier notifier)
        {
            _uow = uow;
            _notifier = notifier;
        }
        public async Task<ResultDTO<bool>> SendRequestAsync(int senderId, int toUserId)
        {
            if (senderId == toUserId)
                return new ResultDTO<bool>(false, "Cannot send a request to yourself.");

            

            // 2) Check for existing pending requests for the target (avoid duplicates)
            var existing = await _uow.ChatRequests.GetPendingRequestAsync(toUserId, 1, 100);
            if (existing != null && existing.Items.Any(r => r.SenderId == senderId))
                return new ResultDTO<bool>(false, "A pending request already exists.");

            // 3) Create entity using public constructor (safe, follows aggregate rules)
            var newRequest = new ChatRequest(senderId, toUserId);

            // 4) Persist via repository + unit of work
            await _uow.ChatRequests.AddAsync(newRequest);

            // IUnitOfWork exposes SaveChangesAsync() (from your snippet) — call it to persist
            await _uow.SaveChangesAsync();
            //#region Create the Notification

            //var notification = new Notification(
            //     toUserId,                   // Receiver (User who gets the notification)
            //      null,                       // ChatId (Null, no chat yet)
            //     newRequest.Id,              // ChatRequestId (Linked!)
            //     "You have a new connection request",
            //      NotificationType.ChatRequestReceived
            //);
            //await _uow.Notifications.AddAsync(notification);
            //await _uow.SaveChangesAsync(); // <--- Persist the notification 
            
            //await _notifier.SendNotificationAsync(toUserId, notification);
            //#endregion

            return new ResultDTO<bool>(true, "Request sent successfully", true);
        }

        public async Task<ResultDTO<object>> ChangeRequestStatusAsync(int userId, int requestId, RequestStatus status)
        {
            // Parallel fetch: request + current user (for notification display)
            var requestTask = _uow.ChatRequests.GetByKeysAsync(requestId);
            var currentUserTask = _uow.Users.GetByKeysAsync(userId);
            
            await Task.WhenAll(requestTask, currentUserTask);
            
            var Request = await requestTask;
            var currentUser = await currentUserTask;
            
            if (Request == null)
                return new ResultDTO<object>(false, "Invalid RequestId", null);
            
            if (currentUser == null)
                return new ResultDTO<object>(false, "User not found", null);
            
            // Authorization: only the receiver can accept/reject
            if (Request.ReceiverId != userId)
                return new ResultDTO<object>(false, "Not authorized to change this request", null);
            
            // Validate status change
            if (status == RequestStatus.Pending)
                return new ResultDTO<object>(false, "Cannot set status to Pending", null);

            var type = NotificationType.ChatRequestReceived;
            string message;
            switch (status)
            {
                case RequestStatus.Accepted:
                    Request.Accept();
                    type = NotificationType.ChatRequestAccepted;
                    message = $"{currentUser.FirstName} {currentUser.LastName} accepted your chat request";
                    break;
                case RequestStatus.Rejected:
                    Request.Reject();
                    type = NotificationType.ChatRequestRejected;
                    message = $"{currentUser.FirstName} {currentUser.LastName} rejected your chat request";
                    break;
                default:
                    return new ResultDTO<object>(false, "Invalid status", null);
            }
            
            _uow.ChatRequests.Update(Request);
            // Keep original constructor order for backward compatibility
            var notification = new Notification(Request.SenderId, userId, null, requestId, message, type);
            await _uow.Notifications.AddAsync(notification);
            await _uow.SaveChangesAsync();
            
            // Send real-time notification to ORIGINAL SENDER
            await _notifier.SendNotificationAsync(Request.SenderId, new NotificationDTO
            {
                UserId = userId,  // Who triggered this (the receiver who accepted/rejected)
                UserFullName = $"{currentUser.FirstName} {currentUser.LastName}",
                AvatarUrl = currentUser.ImageURL,
                CreatedAt = notification.CreatedAt,
                ChatId = notification.ChatId,
                Content = notification.Message,
                notificationType = notification.Type,
                RequestId = notification.ChatRequestId,
                IsRead = notification.IsRead
            });
           
            return new ResultDTO<object>(true, "Request status has been recorded successfully.", null);
        }


    }
}
