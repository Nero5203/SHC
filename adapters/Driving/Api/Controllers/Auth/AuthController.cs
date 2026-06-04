using Microsoft.AspNetCore.Mvc;
using application.Ports.Driven;
using application.Ports.Driven.Auth;
using application.Ports.Driving.Auth;
using application.Dto.Auth;
using Application.UseCases.Auth;

namespace api.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRegisterUserUseCase _registerUserUseCase;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            IRegisterUserUseCase registerUserUseCase,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _registerUserUseCase = registerUserUseCase;
            _passwordHasher = passwordHasher;
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

            // 2. Validate password
            var isValidPassword = _passwordHasher.VerifyPassword(
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
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest("Required user registration fields are missing.");
            }

            await _registerUserUseCase.Execute(
                request.Email,
                request.Password,
                request.Username,
                request.FirstName,
                request.LastName,
                request.PhoneNumber);

            return Ok("User Registered Successfully");
        }
    }
}
