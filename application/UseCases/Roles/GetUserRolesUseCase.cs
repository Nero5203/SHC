using application.Ports.Driven.Roles;
using application.Ports.Driving.Roles;
using Domain.Entities.Roles;

namespace application.UseCases.Roles
{
    public class GetUserRolesUseCase : IGetUserRolesUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public GetUserRolesUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IReadOnlyList<UserRole>> ExecuteAsync(Guid userId)
        {
            return await _roleRepository.GetUserRolesAsync(userId);
        }
    }
}
