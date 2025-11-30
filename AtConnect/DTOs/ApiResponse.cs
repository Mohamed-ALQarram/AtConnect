namespace AtConnect.DTOs
{
    public class ApiResponse<T> where T : class
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }=string.Empty;
        public T? Data { get; set; } 
        public ApiResponse(bool success = false, string? message = "", T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

}
