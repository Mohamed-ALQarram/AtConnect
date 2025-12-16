namespace AtConnect.DTOs.HubDTOs
{
    public record SendMessageRequest(int ReceiverId, int ChatId, string Content); 
}
