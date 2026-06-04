using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;
using Domain.Entities.Roles;

namespace application.UseCases.Roles
{
    public class CreateRoleUseCase : ICreateRoleUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public CreateRoleUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role> ExecuteAsync(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Role name is required.");
            }

            var existingRole = await _roleRepository.GetByNameAsync(name);

            if (existingRole != null)
            {
                throw new ArgumentException("Role with this name already exists.");
            }

            var role = new Role
            {
                RoleId = Guid.NewGuid(),
                Name = name.Trim(),
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await _roleRepository.CreateAsync(role);

            return role;
        }
    }
}
