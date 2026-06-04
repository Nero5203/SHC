using System.Text.Json;
using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.UseCases.Permissions
{
    public class GrantPermissionUseCase : IGrantPermissionUseCase
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public GrantPermissionUseCase(
            IPermissionRepository permissionRepository,
            IAuditLogRepository auditLogRepository)
        {
            _permissionRepository = permissionRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Permission> ExecuteAsync(
            Guid subjectId,
            SubjectType subjectType,
            Guid resourceId,
            ResourceType resourceType,
            AccessLevel accessLevel,
            Guid? grantedBySubjectId,
            SubjectType? grantedBySubjectType)
        {
            if (accessLevel == AccessLevel.None)
            {
                throw new ArgumentException("Use revoke permission instead of granting AccessLevel.None.");
            }

            var now = DateTime.UtcNow;
            var actorId = grantedBySubjectId ?? subjectId;
            var actorType = grantedBySubjectType ?? subjectType;

            var permission = await _permissionRepository.GetForSubjectResourceAsync(
                subjectType,
                subjectId,
                resourceType,
                resourceId);

            if (permission == null)
            {
                permission = new Permission
                {
                    PermissionId = Guid.NewGuid(),
                    SubjectId = subjectId,
                    SubjectType = subjectType,
                    ResourceId = resourceId,
                    ResourceType = resourceType,
                    AccessLevel = accessLevel,
                    GrantedAtUtc = now,
                    GrantedBySubjectId = actorId,
                    GrantedBySubjectType = actorType
                };

                await _permissionRepository.CreateAsync(permission);
            }
            else
            {
                permission.AccessLevel = accessLevel;
                permission.GrantedAtUtc = now;
                permission.GrantedBySubjectId = actorId;
                permission.GrantedBySubjectType = actorType;

                await _permissionRepository.UpdateAsync(permission);
            }

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                AuditLogId = Guid.NewGuid(),
                TimestampUtc = now,
                SubjectId = actorId,
                SubjectType = actorType,
                Action = "Permission.Granted",
                ResourceType = resourceType,
                ResourceId = resourceId.ToString(),
                IsSuccess = true,
                PayloadJson = JsonSerializer.Serialize(new
                {
                    permission.PermissionId,
                    permission.SubjectId,
                    permission.SubjectType,
                    permission.AccessLevel
                })
            });

            return permission;
        }
    }
}
