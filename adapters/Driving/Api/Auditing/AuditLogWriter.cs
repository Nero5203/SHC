using System.Security.Claims;
using System.Text.Json;
using application.Ports.Driven.Permissions;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace api.Auditing
{
    public class AuditLogWriter : IAuditLogWriter
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogWriter(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task WriteAsync(
            ClaimsPrincipal user,
            string action,
            ResourceType resourceType,
            string resourceId,
            bool isSuccess = true,
            object? payload = null)
        {
            var subjectId = GetSubjectId(user);

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                AuditLogId = Guid.NewGuid(),
                TimestampUtc = DateTime.UtcNow,
                SubjectId = subjectId,
                SubjectType = subjectId.HasValue ? SubjectType.User : SubjectType.ServiceAccount,
                Action = action,
                ResourceType = resourceType,
                ResourceId = resourceId,
                IsSuccess = isSuccess,
                PayloadJson = payload == null ? string.Empty : JsonSerializer.Serialize(payload)
            });
        }

        private static Guid? GetSubjectId(ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }
}
