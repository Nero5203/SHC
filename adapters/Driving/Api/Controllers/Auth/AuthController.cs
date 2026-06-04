using Microsoft.AspNetCore.Mvc;
using application.Ports.Driven;
using application.Ports.Driven.Auth;
using application.Ports.Driving.Auth;
using application.Dto.Auth;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public AuthController(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            IRegisterUserUseCase registerUserUseCase,
            ILoginUserUseCase loginUserUseCase,
            IPasswordHasher passwordHasher,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _registerUserUseCase = registerUserUseCase;
            _loginUserUseCase = loginUserUseCase;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var loginRequest = _mapper.Map<LoginUserRequest>(request);
            var token = await _loginUserUseCase.LoginUserAsync(loginRequest);

            if (token == null)
                return Unauthorized("Invalid email or password.");

            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            var registerUserRequest = _mapper.Map<RegisterUserRequest>(request);
            await _registerUserUseCase.RegisterUserAsync(registerUserRequest);
            return Ok("User registered successfully.");
        }
    }
}
