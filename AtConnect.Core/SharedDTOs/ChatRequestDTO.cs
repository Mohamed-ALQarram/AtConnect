namespace AtConnect.Core.SharedDTOs
{
    public class ChatRequestDTO
    {

        public int RequestId { get; set; }
        public int SenderId{ get; set; }
        public string SenderName{ get; set; }
        public string ProfilePhotoUrl { get; set; }
        public int MutualFriends { get; set; }
        public ChatRequestDTO(int requestId, int senderId, string senderName, string profilePhotoUrl, int mutualFriends)
        {
            RequestId = requestId;
            SenderId = senderId;
            SenderName = senderName;
            ProfilePhotoUrl = profilePhotoUrl;
            MutualFriends = mutualFriends;
        }
    }
}
