namespace AtConnect.DTOs
{
    public class LoginRequest
    {
        public string UserNameOrEmail { get; set; }= null!;
        public string PasswordHash { get; set; }= null!;
    }
}
