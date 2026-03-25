using AtConnect.Core.Enum;

namespace AtConnect.Core.SharedDTOs
{
    public record MessageDto(int Id, int SenderId, int ChatId, string content, DateTime SentAt, MessageStatus Status);
}
