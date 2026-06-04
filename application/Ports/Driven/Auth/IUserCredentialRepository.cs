using Domain.Entities.Users;
using Domain.Entities.Auth;

namespace application.Ports.Driven.Auth
{
    public interface IUserCredentialRepository
    {
        Task<UserCredential?> GetByUserIdAsync(Guid userId);
        Task CreateAsync(UserCredential credential);
    }
}