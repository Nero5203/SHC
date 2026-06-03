using Domain.Entities.Users;
using ports.DrivenPorts;
using ports.DrivingPorts.Users;

namespace application.UseCases.Users
{
    public class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public CreateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> ExecuteAsync(
            string username,
            string firstName,
            string lastName,
            string email,
            string? profilePictureUrl,
            string? phoneNumber)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                return null;
            }

            var now = DateTime.UtcNow;

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                ProfilePictureUrl = profilePictureUrl,
                PhoneNumber = phoneNumber,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _userRepository.CreateAsync(user);

            return user;
        }
    }
}
