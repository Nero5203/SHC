using Domain.Entities.Users;

namespace application.UseCases.Users
{
    public interface IUserUseCase
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserSettingsAsync(Guid userId);

        Task<User?> UpdateUserProfileAsync(
            Guid userId,
            string? username,
            string? firstName,
            string? lastName,
            string? profilePictureUrl,
            string? phoneNumber);

        Task<User?> UpdateUserSettingsAsync(
            Guid userId,
            string? theme,
            string? language,
            string? defaultView,
            bool? emailNotifications,
            bool? pushNotifications,
            bool? showProfilePicture,
            bool? showActivityStatus);

        Task<bool> DeleteUserAsync(Guid userId);
    }
}