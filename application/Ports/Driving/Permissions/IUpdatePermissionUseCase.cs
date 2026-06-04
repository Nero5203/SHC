using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driving.Permissions
{
    public interface IUpdatePermissionUseCase
    {
        Task<Permission?> ExecuteAsync(
            Guid permissionId,
            AccessLevel accessLevel,
            Guid? grantedBySubjectId,
            SubjectType? grantedBySubjectType);
    }
}
