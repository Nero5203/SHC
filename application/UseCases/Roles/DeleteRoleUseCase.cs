using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;

namespace application.UseCases.Roles
{
    public class DeleteRoleUseCase : IDeleteRoleUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> ExecuteAsync(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);

            if (role == null)
            {
                return false;
            }

            await _roleRepository.DeleteAsync(role);

            return true;
        }
    }
}
