using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driving.Permissions
{
    public interface IListAuditLogsUseCase
    {
        Task<IReadOnlyList<AuditLog>> ExecuteAsync(
            SubjectType? subjectType,
            Guid? subjectId,
            ResourceType? resourceType,
            Guid? resourceId);
    }
}
