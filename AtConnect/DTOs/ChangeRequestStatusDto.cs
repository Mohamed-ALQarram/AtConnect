namespace AtConnect.DTOs
{
    /// <summary>
    /// Request DTO for changing chat request status
    /// </summary>
    public class ChangeRequestStatusDto
    {
        public int RequestId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
