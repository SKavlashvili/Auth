using AuthGEO.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AuthGEO
{
    public enum JWTHash
    {
        SHA256,
        SHA512
    }
    public class JWTService : IJWTService
    {
        public const string Role = "JWTRole";

        private Dictionary<string, object> _firstPart;
        private string _secretKey;
        private JWTHash _jwtHash;
        private ValidationParameters _validationParameters;
        private string _tokenName;
        public JWTService(string tokenName, string secretKey, JWTHash hashType, ValidationParameters validationParameters)
        {
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 20) 
                                        throw new SecretKeyIsNotInCorrectFormatException();

            _tokenName = tokenName;

            _validationParameters = validationParameters;

            _secretKey = secretKey;

            _jwtHash = hashType;

            _firstPart = new Dictionary<string, object>();
            _firstPart.Add("HashType", hashType.ToString());
            _firstPart.Add("TokenType", "JWT");
        }
        public string GetTokenName()
        {
            return _tokenName;
        }
        public string GenerateJWT(DateTime expireDate, params JWTClaim[] claims)
        {

            Dictionary<string,object> Claims = new Dictionary<string,object>();
            foreach(JWTClaim claim in claims)
            {
                Claims[claim.Key] = claim.Value;
            }

            Claims["ExpireDate"] = expireDate;

            string ClaimsBase64 = ConvertToBase64(JsonSerializer.Serialize(Claims));
            string FirstPartBase64 = ConvertToBase64(JsonSerializer.Serialize(_firstPart));
            return $"{FirstPartBase64}.{ClaimsBase64}.{Hash($"{FirstPartBase64}.{ClaimsBase64}.{_secretKey}")}";
        }

        public bool IsValidJWT(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new TokenIsInIncorrectFormatException();

            string[] splittedToken = token.Split('.');
            if(splittedToken.Length != 3) throw new TokenIsInIncorrectFormatException();

            if (!Hash($"{splittedToken[0]}.{splittedToken[1]}.{_secretKey}").Equals(splittedToken[2])) throw new UnauthorizedException();

            if(TimeMustBeValidated())
            {
                var Payload = JsonSerializer.Deserialize<Dictionary<string, object>>(ConvertFromBase64(splittedToken[1]));
                //ტოკენი თუ ამის დაგენერირებულია, მაშინ შიგნით ExpireDate, აუცილებლად იდება, სხვანაირად ვერ დააგენერირებდა.
                DateTime expireDate = DateTime.Parse(Payload["ExpireDate"].ToString());
                if (expireDate <= DateTime.Now) throw new TokenExpiredException();
            }

            return true;
        }
        public bool TimeMustBeValidated()
        {
            return _validationParameters.ValidateExpireDate;
        }
        public string GetValue(string token, string key)
        {
            string payLoadJson = ConvertFromBase64(token.Split('.')[1]);

            Dictionary<string, object> payLoad = JsonSerializer.Deserialize<Dictionary<string, object>>(payLoadJson);

            if (!payLoad.ContainsKey(key)) throw new AuthGEO.Exceptions.KeyNotFoundException(key);

            return payLoad[key].ToString();
        }
        public List<JWTRole>? GetRequiredRolesIfExists(HttpContext context)
        {
            Microsoft.AspNetCore.Http.Endpoint endPoint = context.Features.Get<IEndpointFeature>().Endpoint;
            if (endPoint == null) return null;

            ControllerActionDescriptor actionDescriptor = endPoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (actionDescriptor == null) return null;

            MethodInfo endPointMethodInfo = actionDescriptor.MethodInfo;


            if (endPointMethodInfo == null) return null;

            List<JWTRole> jwtrAttributes = endPointMethodInfo.GetCustomAttributes(typeof(JWTRole), false)
                               .Cast<JWTRole>().ToList();

            if (jwtrAttributes.Count == 0) return null;

            return jwtrAttributes;
        }

        public bool RoleAuthorizationRequired()
        {
            return _validationParameters.ValidateRole;
        }

        public string GetRoleFromToken(string token)
        {
            string payLoadJson = ConvertFromBase64(token.Split('.')[1]);

            Dictionary<string,object> payLoad = JsonSerializer.Deserialize<Dictionary<string,object>>(payLoadJson);

            if (!payLoad.ContainsKey("JWTRole")) throw new UnauthorizedException();

            return payLoad["JWTRole"].ToString();
        }

        #region StringHelperMethods
        public string Hash(string input)
        {
            if (_jwtHash == JWTHash.SHA256)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);
                    string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    return hashString;
                }
            }
            else if(_jwtHash == JWTHash.SHA512)
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = sha512.ComputeHash(inputBytes);
                    string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    return hashString;
                }
            }
            return null;
        }

        public string ConvertToBase64(string input)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
            string base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        public string ConvertFromBase64(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            string originalString = System.Text.Encoding.UTF8.GetString(bytes);
            return originalString;
        }

        #endregion
    }
}
