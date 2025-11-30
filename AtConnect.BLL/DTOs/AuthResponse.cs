namespace AtConnect.BLL.DTOs
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }=string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt  { get; set; } 
        public DateTime RefreshTokenExpiresAt  { get; set; }
    }
}

