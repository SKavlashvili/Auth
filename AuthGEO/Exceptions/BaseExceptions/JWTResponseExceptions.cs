namespace AuthGEO.Exceptions
{
    public class JWTResponseExceptions : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public JWTResponseExceptions(string message, int statusCode) : base(message)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
