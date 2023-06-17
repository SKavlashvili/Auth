using AuthGEO.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AuthGEO
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJWTService _jwtInfo;
        public AuthenticationMiddleware(RequestDelegate next, IJWTService jwtInfo)
        {
            _next = next;
            _jwtInfo = jwtInfo;
        }


        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                if(!context.Request.Headers.ContainsKey(_jwtInfo.GetTokenName()))
                {
                    await _next(context);
                    return;
                }

                string tokenFromHeader = context.Request.Headers[_jwtInfo.GetTokenName()];

                if(_jwtInfo.IsValidJWT(tokenFromHeader)) await _next(context);
            }
            catch(JWTResponseExceptions error)
            {
                context.Response.StatusCode = error.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.
                    WriteAsync(JsonSerializer.Serialize(new { StatusCode = error.StatusCode, Message = error.Message }));
            }
        }

    }
}
