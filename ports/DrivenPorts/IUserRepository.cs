using Domain.Entities.Users;

namespace ports.DrivenPorts
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId);
        Task<User?> GetWithSettingsByIdAsync(Guid userId);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> ExistsByIdAsync(Guid userId);
    }
}