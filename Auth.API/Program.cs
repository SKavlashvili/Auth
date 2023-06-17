using AuthGEO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

//JWT configurations
builder.Services.AddJWTService
    (
            new JWTService
            (
                "Auth", 
                "safldajglksgslagsalkgslak", 
                JWTHash.SHA256,
                new ValidationParameters()
                {
                    ValidateExpireDate = true,
                    ValidateRole = true,
                }
            )
    );


var app = builder.Build();


app.UseRouting();//ეს საჭიროა, რათა Context-ში EndPoint-ის ინსტანსი null არ იყოს და ამოღება შესაძლებელი
//გახდეს.

//Auth
app.UseAuthenticationGEO();
app.AddAuthorizationGEO();

app.UseEndpoints((IEndpointRouteBuilder endPoints) =>
{
    endPoints.MapControllers();
});

app.Run();
