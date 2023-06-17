namespace AuthGEO.Exceptions
{
    public class SecretKeyIsNotInCorrectFormatException : JWTBaseException
    {
        public SecretKeyIsNotInCorrectFormatException() : base("Secret key requirements: minLength = 20")
        {

        }
    }

}
