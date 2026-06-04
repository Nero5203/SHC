using Domain.Entities.Users;

namespace application.Ports.Driving
{
    public interface IUpdateUserProfileUseCase
    {
        Task<User?> ExecuteAsync(
            Guid userId,
            string? username,
            string? firstName,
            string? lastName,
            string? profilePictureUrl,
            string? phoneNumber);
    }
}
