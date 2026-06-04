using System.Text.Json;
using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.UseCases.Permissions
{
    public class CheckPermissionUseCase : ICheckPermissionUseCase
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public CheckPermissionUseCase(
            IPermissionRepository permissionRepository,
            IAuditLogRepository auditLogRepository)
        {
            _permissionRepository = permissionRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<(bool HasAccess, AccessLevel? GrantedAccessLevel)> ExecuteAsync(
            Guid subjectId,
            SubjectType subjectType,
            Guid resourceId,
            ResourceType resourceType,
            AccessLevel requiredAccessLevel)
        {
            var permission = await _permissionRepository.GetForSubjectResourceAsync(
                subjectType,
                subjectId,
                resourceType,
                resourceId);

            var hasAccess = requiredAccessLevel == AccessLevel.None ||
                permission != null && permission.AccessLevel >= requiredAccessLevel;

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                AuditLogId = Guid.NewGuid(),
                TimestampUtc = DateTime.UtcNow,
                SubjectId = subjectId,
                SubjectType = subjectType,
                Action = "Permission.Checked",
                ResourceType = resourceType,
                ResourceId = resourceId.ToString(),
                IsSuccess = hasAccess,
                PayloadJson = JsonSerializer.Serialize(new
                {
                    RequiredAccessLevel = requiredAccessLevel,
                    GrantedAccessLevel = permission?.AccessLevel
                })
            });

            return (hasAccess, permission?.AccessLevel);
        }
    }
}
