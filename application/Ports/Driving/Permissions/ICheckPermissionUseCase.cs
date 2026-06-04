using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driving.Permissions
{
    public interface ICheckPermissionUseCase
    {
        Task<(bool HasAccess, AccessLevel? GrantedAccessLevel)> ExecuteAsync(
            Guid subjectId,
            SubjectType subjectType,
            Guid resourceId,
            ResourceType resourceType,
            AccessLevel requiredAccessLevel);
    }
}
