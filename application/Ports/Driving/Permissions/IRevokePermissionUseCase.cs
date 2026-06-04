using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driving.Permissions
{
    public interface IRevokePermissionUseCase
    {
        Task<bool> ExecuteAsync(
            Guid permissionId,
            Guid? revokedBySubjectId,
            SubjectType? revokedBySubjectType);
    }
}
