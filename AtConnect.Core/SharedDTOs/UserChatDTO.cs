namespace AtConnect.Core.SharedDTOs
{
    public class UserChatDTO
    {
        public int UserId { get; set; }
        public string OtherUserImageURL { get; set; } = null!;
        public bool OtherUserIsActive { get; set; }
        public string MostRecentMessageContent { get; set; } = null!;
        public DateTime MostRecentMessageSentAt { get; set; }
        public int UnreadMessageCount { get; set; }
        
        public UserChatDTO(int otherUserId, string otherUserImageURL, bool otherUserIsActive, 
                          string mostRecentMessageContent, DateTime mostRecentMessageSentAt, int unreadMessageCount)
        {
            UserId = otherUserId;
            OtherUserImageURL = otherUserImageURL;
            OtherUserIsActive = otherUserIsActive;
            MostRecentMessageContent = mostRecentMessageContent;
            MostRecentMessageSentAt = mostRecentMessageSentAt;
            UnreadMessageCount = unreadMessageCount;
        }
    }
}
