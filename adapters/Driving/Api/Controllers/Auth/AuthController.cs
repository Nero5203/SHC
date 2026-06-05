using Microsoft.AspNetCore.Mvc;
using application.Ports.Driven;
using application.Ports.Driven.Auth;
using application.Ports.Driving.Auth;
using application.Dto.Auth;
using application.UseCases.Auth;

namespace api.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRegisterUserUseCase _registerUserUseCase;
        private readonly ILoginUserUseCase _loginUserUseCase;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogoutUserUseCase _logoutUserUseCase;

        public AuthController(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            IRegisterUserUseCase registerUserUseCase,
            ILoginUserUseCase loginUserUseCase,
            IPasswordHasher passwordHasher,
            ILogoutUserUseCase logoutUserUseCase)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _registerUserUseCase = registerUserUseCase;
            _loginUserUseCase = loginUserUseCase;
            _passwordHasher = passwordHasher;
            _logoutUserUseCase = logoutUserUseCase;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var loginRequest = new LoginUserRequest
            {
                Email = request.Email,
                Password = request.Password
            };

            var token = await _loginUserUseCase.LoginUserAsync(loginRequest);

            if (token == null)
                return Unauthorized("Invalid email or password.");

            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            var registerUserRequest = new RegisterUserRequest
            {
                Email = request.Email,
                Password = request.Password,
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            await _registerUserUseCase.RegisterUserAsync(registerUserRequest);
            return Ok("User registered successfully.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutDto request)
        {
            var logoutRequest = new LogoutRequest
            {
                RefreshToken = request.RefreshToken
            };

            await _logoutUserUseCase.LogoutUserAsync(logoutRequest);
            return Ok("User logged out successfully.");
        }
    }
}
