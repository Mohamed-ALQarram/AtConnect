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
            #region Create the Notification

            var notification = new Notification(
                 toUserId,                   // Receiver (User who gets the notification)
                  null,                       // ChatId (Null, no chat yet)
                 newRequest.Id,              // ChatRequestId (Linked!)
                 "You have a new connection request",
                  NotificationType.ChatRequestReceived
            );
            await _uow.Notifications.AddAsync(notification);
            await _uow.SaveChangesAsync(); // <--- Persist the notification 
            
            await _notifier.SendNotificationAsync(toUserId, notification);
            #endregion

            return new ResultDTO<bool>(true, "Request sent successfully", true);
        }
    }
}
