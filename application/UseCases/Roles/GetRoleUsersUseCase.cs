using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;
using Domain.Entities.Roles;

namespace application.UseCases.Roles
{
    public class GetRoleUsersUseCase : IGetRoleUsersUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public GetRoleUsersUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IReadOnlyList<UserRole>> ExecuteAsync(Guid roleId)
        {
            return await _roleRepository.GetRoleUsersAsync(roleId);
        }
    }
}
