namespace AuthGEO.Exceptions
{
    public class KeyNotFoundException : JWTBaseException
    {
        public KeyNotFoundException(string key) : base($"key:{key} not found")
        {

        }
    }
}
