namespace Application.UseCases.Auth
{
    using Domain.Entities.Auth;
    using Domain.Entities.Users;
    using ports.DrivenPorts;
    using ports.DrivenPorts.Auth;

    public class RegisterUserUseCase
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserCredentialRepository _credentialRepo;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserUseCase(
            IUserRepository userRepo,
            IUserCredentialRepository credentialRepo,
            IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _credentialRepo = credentialRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task Execute(
            string email,
            string password,
            string username,
            string firstName,
            string lastName,
            string phoneNumber)
        {
            var existingUser = await _userRepo.GetByEmailAsync(email);

            if (existingUser != null)
                throw new Exception("User already exists");

            var userId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                Email = email,
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };

            var credential = new UserCredential
            {
                UserCredentialId = Guid.NewGuid(),
                UserId = userId,
                PasswordHash = _passwordHasher.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.CreateAsync(user);
            await _credentialRepo.CreateAsync(credential);
        }
    }
}
