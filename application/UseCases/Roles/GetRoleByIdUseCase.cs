using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;
using Domain.Entities.Roles;

namespace application.UseCases.Roles
{
    public class GetRoleByIdUseCase : IGetRoleByIdUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public GetRoleByIdUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role?> ExecuteAsync(Guid roleId)
        {
            return await _roleRepository.GetByIdAsync(roleId);
        }
    }
}
