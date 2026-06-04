using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;
using Domain.Entities.Roles;

namespace application.UseCases.Roles
{
    public class AssignRoleToUserUseCase : IAssignRoleToUserUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public AssignRoleToUserUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<UserRole?> ExecuteAsync(Guid roleId, Guid userId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            var userExists = await _roleRepository.UserExistsAsync(userId);

            if (role == null || !userExists)
            {
                return null;
            }

            var existingUserRole = await _roleRepository.GetUserRoleAsync(userId, roleId);

            if (existingUserRole != null)
            {
                return existingUserRole;
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            };

            await _roleRepository.AssignRoleToUserAsync(userRole);

            return await _roleRepository.GetUserRoleAsync(userId, roleId) ?? userRole;
        }
    }
}
