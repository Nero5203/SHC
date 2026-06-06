using application.Dto.Auth;
using application.Ports.Driven;
using application.Ports.Driven.Auth;
using application.Ports.Driving.Auth;
using application.UseCases.Auth;

namespace Application.UseCases.Auth
{
    public class LoginUserUseCase : ILoginUserUseCase
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserCredentialRepository _credentialRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;

        public LoginUserUseCase(
            IUserRepository userRepo,
            IUserCredentialRepository credentialRepo,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator)
        {
            _userRepo = userRepo;
            _credentialRepo = credentialRepo;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponseDto?> LoginUserAsync(LoginUserRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var credential = await _credentialRepo.GetByUserIdAsync(user.UserId);

            if (credential == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var isValid = _passwordHasher.VerifyPassword(
                request.Password,
                credential.PasswordHash);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = await _tokenGenerator.GenerateTokenAsync(user);

            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            return new LoginResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken
            };
        }
    }
}
