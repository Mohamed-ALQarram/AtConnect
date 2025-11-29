using AtConnect.BLL.DTOs;

namespace AtConnect.DTOs
{
    public class TokenResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }=string.Empty;
        public TokenDTO Tokens { get; set; } = new TokenDTO();
        public TokenResponse(bool success, string message, TokenDTO tokens)
        {
            Success = success;
            Message = message;
            Tokens = tokens;
        }
    }
}
