using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driving.Permissions
{
    public interface IGrantPermissionUseCase
    {
        Task<Permission> ExecuteAsync(
            Guid subjectId,
            SubjectType subjectType,
            Guid resourceId,
            ResourceType resourceType,
            AccessLevel accessLevel,
            Guid? grantedBySubjectId,
            SubjectType? grantedBySubjectType);
    }
}
