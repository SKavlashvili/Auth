using AuthGEO.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text.Json;

namespace AuthGEO.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJWTService _jwtService;
        public AuthorizationMiddleware(RequestDelegate next, IJWTService jwtService)
        {
            this._next = next;
            _jwtService = jwtService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if(!_jwtService.RoleAuthorizationRequired())
                {
                    await _next(context);
                    return;
                }

                List<JWTRole> roles = _jwtService.GetRequiredRolesIfExists(context);
                if (roles == null) //role is not required for mapped endpoint
                {
                    await _next(context);
                    return;
                }

                if (!context.Request.Headers.ContainsKey(_jwtService.GetTokenName()))
                {
                    throw new UnauthorizedException();
                }

                //თუ ტოკენს შეიცავს, ესეიგი ტოკენი ვალიდრუია, რადგან მანამდე Authentication მიდლევეარი
                //აქვს უკვე გავლილი
                string token = context.Request.Headers[_jwtService.GetTokenName()];

                string tokenInsertedRole = _jwtService.GetRoleFromToken(token);

                for(int i = 0; i < roles.Count; i++)
                {
                    if (roles[i].Role.Equals(tokenInsertedRole))
                    {
                        await _next(context);
                        return;
                    }
                }
                throw new UnauthorizedException();
            }
            catch(JWTResponseExceptions ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.
                    WriteAsync(JsonSerializer.Serialize(new { StatusCode = ex.StatusCode, Message = ex.Message }));
            }
        }
    }
}
