using Domain.Entities.Users;

namespace ports.DrivingPorts
{
    public interface ICreateUserUseCase
    {
        Task<User?> ExecuteAsync(
            string username,
            string firstName,
            string lastName,
            string email,
            string? profilePictureUrl,
            string? phoneNumber);
    }
}
