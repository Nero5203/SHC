using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.UseCases.Permissions
{
    public class GetPermissionsByResourceUseCase : IGetPermissionsByResourceUseCase
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionsByResourceUseCase(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IReadOnlyList<Permission>> ExecuteAsync(ResourceType resourceType, Guid resourceId)
        {
            return await _permissionRepository.GetByResourceAsync(resourceType, resourceId);
        }
    }
}
