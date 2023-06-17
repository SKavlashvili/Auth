using AuthGEO.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace AuthGEO
{
    public static class AuthorizationExtensions
    {
        public static IApplicationBuilder AddAuthorizationGEO(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}
