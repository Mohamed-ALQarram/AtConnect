namespace AtConnect.BLL.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get;  set; } = string.Empty;
        public TimeSpan JwtLifeTime { get;set; }
        public double RefreshTokenLifeTime { get;set; }
        public string SigningKey { get; set; } = string.Empty;
    }
}
