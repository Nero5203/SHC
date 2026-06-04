using System.Text.Json;
using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.UseCases.Permissions
{
    public class RevokePermissionUseCase : IRevokePermissionUseCase
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public RevokePermissionUseCase(
            IPermissionRepository permissionRepository,
            IAuditLogRepository auditLogRepository)
        {
            _permissionRepository = permissionRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<bool> ExecuteAsync(
            Guid permissionId,
            Guid? revokedBySubjectId,
            SubjectType? revokedBySubjectType)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);

            if (permission == null)
            {
                return false;
            }

            var now = DateTime.UtcNow;
            var actorId = revokedBySubjectId ?? permission.GrantedBySubjectId ?? permission.SubjectId;
            var actorType = revokedBySubjectType ?? permission.GrantedBySubjectType;

            await _permissionRepository.DeleteAsync(permission);

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                AuditLogId = Guid.NewGuid(),
                TimestampUtc = now,
                SubjectId = actorId,
                SubjectType = actorType,
                Action = "Permission.Revoked",
                ResourceType = permission.ResourceType,
                ResourceId = permission.ResourceId.ToString(),
                IsSuccess = true,
                PayloadJson = JsonSerializer.Serialize(new
                {
                    permission.PermissionId,
                    permission.SubjectId,
                    permission.SubjectType,
                    permission.AccessLevel
                })
            });

            return true;
        }
    }
}
