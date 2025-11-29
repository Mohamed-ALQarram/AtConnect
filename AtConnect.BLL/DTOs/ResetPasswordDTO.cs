namespace AtConnect.BLL.DTOs
{
    public record ForgotPasswordDTO(string Email);

    public record VerifyResetTokenDTO(string Email, string Token);

    public record ResetPasswordDTO(string Email, string Token, string NewPassword);
    public record VerifyResetTokenRequest(string RequestToken, string StoredToken, DateTime? ExpireDate);

}
