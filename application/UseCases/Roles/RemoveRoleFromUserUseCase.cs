using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;

namespace application.UseCases.Roles
{
    public class RemoveRoleFromUserUseCase : IRemoveRoleFromUserUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public RemoveRoleFromUserUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> ExecuteAsync(Guid roleId, Guid userId)
        {
            var userRole = await _roleRepository.GetUserRoleAsync(userId, roleId);

            if (userRole == null)
            {
                return false;
            }

            await _roleRepository.RemoveRoleFromUserAsync(userRole);

            return true;
        }
    }
}
