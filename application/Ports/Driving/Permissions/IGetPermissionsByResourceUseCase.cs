using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driving.Permissions
{
    public interface IGetPermissionsByResourceUseCase
    {
        Task<IReadOnlyList<Permission>> ExecuteAsync(ResourceType resourceType, Guid resourceId);
    }
}
