using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using SHC.Domain.Entities.Permissions;

namespace application.UseCases.Permissions
{
    public class GetAllPermissionsUseCase : IGetAllPermissionsUseCase
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetAllPermissionsUseCase(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IReadOnlyList<Permission>> ExecuteAsync()
        {
            return await _permissionRepository.GetAllAsync();
        }
    }
}
