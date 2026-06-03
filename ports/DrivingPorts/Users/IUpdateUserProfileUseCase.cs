using Domain.Entities.Users;

namespace ports.DrivingPorts
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
