namespace AuthGEO.Exceptions
{
    public class TokenIsInIncorrectFormatException : JWTResponseExceptions
    {
        public TokenIsInIncorrectFormatException() : base("Token is in incorrect format",400)
        {

        }
    }
}
