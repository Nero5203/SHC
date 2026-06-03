using Domain.Entities.Users;
using Domain.Entities.Auth;

namespace ports.DrivenPorts
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid userId);
        Task<User?> GetWithSettingsByIdAsync(Guid userId);
            Task<UserCredential?> GetCredentialsByEmailAsync(string email);

        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> ExistsByIdAsync(Guid userId);
    }
}