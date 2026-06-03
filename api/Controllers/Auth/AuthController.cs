using Microsoft.AspNetCore.Mvc;
using ports.DrivenPorts;
using ports.DrivenPorts.Auth;
using BCrypt.Net;
using api.Dto.Auth;

namespace api.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthController(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            // 0. Validate request
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required.");
            }

            // 1. Get credentials (password hash lives here)
            var credentials = await _userRepository.GetCredentialsByEmailAsync(request.Email);

            if (credentials == null)
                return Unauthorized("Invalid credentials");

            // 2. Validate password (BCrypt)
            var isValidPassword = BCrypt.Net.BCrypt.Verify(
                request.Password,
                credentials.PasswordHash
            );

            if (!isValidPassword)
                return Unauthorized("Invalid credentials");

            // 3. Get full user (for JWT claims)
            var user = await _userRepository.GetByIdAsync(credentials.UserId);

            if (user == null)
                return Unauthorized("Invalid credentials");

            // 4. Generate JWT
            var token = _tokenGenerator.GenerateToken(user);

            return Ok(new
            {
                accessToken = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            return Ok("User Registered Successfully");
        }
    }
}