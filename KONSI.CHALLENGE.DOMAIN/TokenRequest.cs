namespace KONSI.CHALLENGE.DOMAIN
{
    public class TokenRequest
    {
        public string Success { get; set; }
        public TokenData Data { get; set; }
    }

    public class TokenData
    {
        public string Token { get; set; }
        public string Type { get; set; }
        public string ExpiresIn { get; set; }
    }
}