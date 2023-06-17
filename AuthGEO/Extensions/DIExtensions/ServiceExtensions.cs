
using Microsoft.Extensions.DependencyInjection;

namespace AuthGEO
{
    public static class ServiceExtensions
    {
        public static void AddJWTService(this IServiceCollection collection, JWTService jwtService)
        {
            collection.AddSingleton<IJWTService>((IServiceProvider provider) =>
            {
                return jwtService;
            });
        }
    }
}
