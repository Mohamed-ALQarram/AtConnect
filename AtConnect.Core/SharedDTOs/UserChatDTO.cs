namespace AtConnect.Core.SharedDTOs
{
    public class UserChatDTO
    {
        public int ChatId { get; set; }
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; }

        public string AvatarURL { get; set; } = null!;
        public bool IsActive { get; set; }
        public string MostRecentMessageContent { get; set; } = null!;
        public DateTime MostRecentMessageSentAt { get; set; }
        public int UnreadMessageCount { get; set; }
        
        public UserChatDTO(int ChatId,  int otherUserId, string OtherUserName, string otherUserImageURL, bool otherUserIsActive, 
                          string mostRecentMessageContent, DateTime mostRecentMessageSentAt, int unreadMessageCount)
        {
            this.ChatId = ChatId;
            OtherUserId = otherUserId;
            this.OtherUserName = OtherUserName;
            AvatarURL = otherUserImageURL;
            IsActive = otherUserIsActive;
            MostRecentMessageContent = mostRecentMessageContent;
            MostRecentMessageSentAt = mostRecentMessageSentAt;
            UnreadMessageCount = unreadMessageCount;

        }
    }
}
