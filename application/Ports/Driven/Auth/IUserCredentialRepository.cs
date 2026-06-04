using Domain.Entities.Users;
using Domain.Entities.Auth;

namespace application.Ports.Driven.Auth
{
    public interface IUserCredentialRepository
    {
        Task<bool> UserExistsAsync(string email);
        Task CreateAsync(UserCredential credential);
        Task<User?> GetByEmailAsync(string email);
    }
}