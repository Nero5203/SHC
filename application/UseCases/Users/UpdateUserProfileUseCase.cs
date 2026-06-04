using Domain.Entities.Users;
using application.Ports.Driven;
using application.Ports.Driving;

namespace application.UseCases.Users
{
    public class UpdateUserProfileUseCase : IUpdateUserProfileUseCase
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> ExecuteAsync(
            Guid userId,
            string? username,
            string? firstName,
            string? lastName,
            string? profilePictureUrl,
            string? phoneNumber)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (username != null)
            {
                user.Username = username;
            }

            if (firstName != null)
            {
                user.FirstName = firstName;
            }

            if (lastName != null)
            {
                user.LastName = lastName;
            }

            if (profilePictureUrl != null)
            {
                user.ProfilePictureUrl = profilePictureUrl;
            }

            if (phoneNumber != null)
            {
                user.PhoneNumber = phoneNumber;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return user;
        }
    }
}
