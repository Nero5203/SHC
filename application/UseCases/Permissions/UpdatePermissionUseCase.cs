using System.Text.Json;
using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.UseCases.Permissions
{
    public class UpdatePermissionUseCase : IUpdatePermissionUseCase
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public UpdatePermissionUseCase(
            IPermissionRepository permissionRepository,
            IAuditLogRepository auditLogRepository)
        {
            _permissionRepository = permissionRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Permission?> ExecuteAsync(
            Guid permissionId,
            AccessLevel accessLevel,
            Guid? grantedBySubjectId,
            SubjectType? grantedBySubjectType)
        {
            if (accessLevel == AccessLevel.None)
            {
                throw new ArgumentException("Use revoke permission instead of updating to AccessLevel.None.");
            }

            var permission = await _permissionRepository.GetByIdAsync(permissionId);

            if (permission == null)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var actorId = grantedBySubjectId ?? permission.GrantedBySubjectId ?? permission.SubjectId;
            var actorType = grantedBySubjectType ?? permission.GrantedBySubjectType;

            permission.AccessLevel = accessLevel;
            permission.GrantedAtUtc = now;
            permission.GrantedBySubjectId = actorId;
            permission.GrantedBySubjectType = actorType;

            await _permissionRepository.UpdateAsync(permission);

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                AuditLogId = Guid.NewGuid(),
                TimestampUtc = now,
                SubjectId = actorId,
                SubjectType = actorType,
                Action = "Permission.Updated",
                ResourceType = permission.ResourceType,
                ResourceId = permission.ResourceId.ToString(),
                IsSuccess = true,
                PayloadJson = JsonSerializer.Serialize(new
                {
                    permission.PermissionId,
                    permission.AccessLevel
                })
            });

            return permission;
        }
    }
}
