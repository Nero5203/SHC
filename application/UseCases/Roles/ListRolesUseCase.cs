using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;
using Domain.Entities.Roles;

namespace application.UseCases.Roles
{
    public class ListRolesUseCase : IListRolesUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public ListRolesUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IReadOnlyList<Role>> ExecuteAsync()
        {
            return await _roleRepository.GetAllAsync();
        }
    }
}
