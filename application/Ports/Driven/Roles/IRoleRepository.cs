using Domain.Entities.Roles;

namespace application.Ports.Driven.Roles
{
    public interface IRoleRepository
    {
        Task CreateAsync(Role role);
        Task<Role?> GetByIdAsync(Guid roleId);
        Task<Role?> GetByNameAsync(string name);
        Task<IReadOnlyList<Role>> GetAllAsync();
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task<bool> UserExistsAsync(Guid userId);
        Task<UserRole?> GetUserRoleAsync(Guid userId, Guid roleId);
        Task AssignRoleToUserAsync(UserRole userRole);
        Task RemoveRoleFromUserAsync(UserRole userRole);
        Task<IReadOnlyList<UserRole>> GetUserRolesAsync(Guid userId);
        Task<IReadOnlyList<UserRole>> GetRoleUsersAsync(Guid roleId);
    }
}
