namespace AuthGEO
{
    public class JWTClaim
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public JWTClaim(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
