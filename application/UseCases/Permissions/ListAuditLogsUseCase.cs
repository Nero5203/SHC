using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.UseCases.Permissions
{
    public class ListAuditLogsUseCase : IListAuditLogsUseCase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public ListAuditLogsUseCase(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IReadOnlyList<AuditLog>> ExecuteAsync(
            SubjectType? subjectType,
            Guid? subjectId,
            ResourceType? resourceType,
            Guid? resourceId)
        {
            if (subjectType.HasValue && subjectId.HasValue)
            {
                return await _auditLogRepository.GetBySubjectAsync(subjectType.Value, subjectId.Value);
            }

            if (resourceType.HasValue && resourceId.HasValue)
            {
                return await _auditLogRepository.GetByResourceAsync(resourceType.Value, resourceId.Value);
            }

            return await _auditLogRepository.GetAllAsync();
        }
    }
}
