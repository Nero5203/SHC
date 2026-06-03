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
        private readonly IEmailSender _emailSender;

        public RegisterUserUseCase(
            IUserRepository userRepo,
            IUserCredentialRepository credentialRepo,
            IPasswordHasher passwordHasher,
            IEmailSender emailSender)
        {
            _userRepo = userRepo;
            _credentialRepo = credentialRepo;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
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
                PhoneNumber = phoneNumber,
                IsEmailVerified = false
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

            await _emailSender.SendEmailAsync(email, "Verify your email", "verification link");
        }
    }
}
