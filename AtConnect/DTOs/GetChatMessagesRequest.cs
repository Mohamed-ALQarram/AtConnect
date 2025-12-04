namespace AtConnect.DTOs
{
    /// <summary>
    /// Request DTO for getting chat messages with pagination
    /// </summary>
    public class GetChatMessagesRequest
    {
        public int ChatId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
