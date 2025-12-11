namespace AtConnect.DTOs
{
    /// <summary>
    /// Request DTO for getting chat messages with pagination
    /// </summary>
    public class GetChatMessagesRequest
    {
        public int ChatId { get; set; }

        private const int MaxPageSize = 20;
        private int _pageSize = 10;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
