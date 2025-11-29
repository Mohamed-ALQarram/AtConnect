namespace AtConnect.BLL.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get;  set; } = string.Empty;
        public TimeSpan LifeTime { get;set; }
        public string SigningKey { get; set; } = string.Empty;
    }
}
