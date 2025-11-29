namespace AtConnect.BLL.DTOs
{
    public record ResetPasswordRequest(string Email, string Token, string NewPassword);

}
