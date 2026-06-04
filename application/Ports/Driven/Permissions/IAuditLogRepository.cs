using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driven.Permissions
{
    public interface IAuditLogRepository
    {
        Task CreateAsync(AuditLog auditLog);
        Task<IReadOnlyList<AuditLog>> GetAllAsync();
        Task<IReadOnlyList<AuditLog>> GetBySubjectAsync(SubjectType subjectType, Guid subjectId);
        Task<IReadOnlyList<AuditLog>> GetByResourceAsync(ResourceType resourceType, Guid resourceId);
    }
}
