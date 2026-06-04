using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;
using Domain.Entities.Roles;

namespace application.UseCases.Roles
{
    public class UpdateRoleUseCase : IUpdateRoleUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role?> ExecuteAsync(Guid roleId, string? name, string? description)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);

            if (role == null)
            {
                return null;
            }

            if (name != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Role name cannot be empty.");
                }

                var existingRole = await _roleRepository.GetByNameAsync(name);

                if (existingRole != null && existingRole.RoleId != roleId)
                {
                    throw new ArgumentException("Role with this name already exists.");
                }

                role.Name = name.Trim();
            }

            if (description != null)
            {
                role.Description = description;
            }

            await _roleRepository.UpdateAsync(role);

            return role;
        }
    }
}
