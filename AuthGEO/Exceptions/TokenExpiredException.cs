
namespace AuthGEO.Exceptions
{
    public class TokenExpiredException : JWTResponseExceptions
    {
        public TokenExpiredException() : base("Token has been expired",401)
        {

        }
    }
}
