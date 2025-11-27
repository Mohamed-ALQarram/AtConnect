using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.Core.Enum
{
    public enum NotificationType
    {
        NewMessage,               // User received a new chat message
        ChatRequestReceived,      // Someone sent you a chat request
        ChatRequestAccepted,      // Your chat request was accepted
        ChatRequestRejected       // Your chat request was rejected
    }
}
