namespace AtConnect.DTOs
{
    public class ResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }=string.Empty;
        public ResponseDTO(bool success, string message)
        {
            Success = success;
            Message = message;
        }

    }
}
