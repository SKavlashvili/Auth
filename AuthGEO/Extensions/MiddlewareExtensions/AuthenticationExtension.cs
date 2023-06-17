using Microsoft.AspNetCore.Builder;

namespace AuthGEO
{
    public static class AuthenticationExtension
    {
        public static IApplicationBuilder UseAuthenticationGEO(this IApplicationBuilder app)
        {
            /*
             AuthenticationMiddleware მხოლოდ აკეთებს JWT ტოკენის აუთენტიპიკაციას, ასეთის არსებობის
            შემთხვევაში. თუ header-ში არ გვაქვს JWT ტოკენი, მაშინ უბრალოდ გაატარებს და გადავა
            შემდეგ Middleware-ზე, სადაც საჭიროების შემთხვევაში Authorization-მა უნდა მიხედოს
            იმ case-ს, როცა ჰედერში ტოკენი არ არის.
             */
            return app.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
