namespace AtConnect.BLL.DTOs
{
    public record ValidateTokenRequest(string RequestToken, string StoredToken, DateTime? ExpireDate);

}
