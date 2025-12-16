namespace AtConnect.Core.SharedDTOs
{
    public class UserChatDTO
    {
        public int OtherUserId { get; set; }
        public string AvatarURL { get; set; } = null!;
        public bool IsActive { get; set; }
        public string MostRecentMessageContent { get; set; } = null!;
        public DateTime MostRecentMessageSentAt { get; set; }
        public int UnreadMessageCount { get; set; }
        
        public UserChatDTO(int otherUserId, string otherUserImageURL, bool otherUserIsActive, 
                          string mostRecentMessageContent, DateTime mostRecentMessageSentAt, int unreadMessageCount)
        {
            OtherUserId = otherUserId;
            AvatarURL = otherUserImageURL;
            IsActive = otherUserIsActive;
            MostRecentMessageContent = mostRecentMessageContent;
            MostRecentMessageSentAt = mostRecentMessageSentAt;
            UnreadMessageCount = unreadMessageCount;
        }
    }
}
