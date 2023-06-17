using Microsoft.AspNetCore.Http;

namespace AuthGEO
{
    public interface IJWTService
    {
        string GenerateJWT(DateTime expireDate, params JWTClaim[] claims);

        bool IsValidJWT(string token);

        bool TimeMustBeValidated();

        string GetTokenName();

        List<JWTRole> GetRequiredRolesIfExists(HttpContext context);

        string GetRoleFromToken(string token);

        bool RoleAuthorizationRequired();

        string GetValue(string token, string key);//Gets value from payload and casts it
    }
}
