using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts;
using PostGresAPI.Services;
using System.Threading.Tasks;

namespace PostGresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthService _authService;

        public UserAuthController(IUserAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserAuthResultDto>> Register([FromBody] UserRegisterDto registerDto)
        {
            var (success, error, result) = await _authService.Register(registerDto);

            if (!success)
                return BadRequest(new { error });

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserAuthResultDto>> Login([FromBody] UserLoginDto loginDto)
        {
            var (success, error, result) = await _authService.Login(loginDto);

            if (!success)
                return Unauthorized(new { error });

            return Ok(result);
        }
    }
}
