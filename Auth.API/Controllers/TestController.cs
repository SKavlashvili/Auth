using AuthGEO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IJWTService _jWTService;
        public TestController(IJWTService jwtService)
        {
            this._jWTService = jwtService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetToken()
        {
            return Ok(_jWTService.GenerateJWT(DateTime.Now.AddMinutes(2), new JWTClaim(JWTService.Role,"Admin"),new JWTClaim("Name","Test")));
        }

        [JWTRole("Admin")]
        [HttpGet("[action]")]
        public async Task<IActionResult> AdminnedData([FromHeader] string Auth)
        {
            Console.WriteLine(_jWTService.GetValue(Auth, "Name"));
            return Ok();
        }
    }
}
