namespace AuthGEO.Exceptions
{
    public class UnauthorizedException : JWTResponseExceptions
    {
        public UnauthorizedException() : base("Token is not Authorized",401)
        {

        }
    }
}
