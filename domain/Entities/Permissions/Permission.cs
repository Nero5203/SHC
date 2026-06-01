using System;
using SHC.Domain.Entities.Permissions.Enums;

namespace SHC.Domain.Entities.Permissions
{
    public class Permission
    {
        public Guid PermissionId { get; private set; }

        // The Actor
        public Guid SubjectId { get; private set; }
        public SubjectType SubjectType { get; private set; }

        // The Target
        public Guid ResourceId { get; private set; }
        public ResourceType ResourceType { get; private set; }

        // The Capability
        public AccessLevel AccessLevel { get; private set; }

        // Metadata
        public DateTime GrantedAtUtc { get; private set; }
        public Guid? GrantedByUserId { get; private set; }

        // EF Core Constructor
        private Permission() { }

        // Master Constructor
        public Permission(
            Guid subjectId,
            SubjectType subjectType,
            Guid resourceId,
            ResourceType resourceType,
            AccessLevel accessLevel,
            Guid? grantedByUserId)
        {
            Id = Guid.NewGuid();
            SubjectId = subjectId;
            SubjectType = subjectType;
            ResourceId = resourceId;
            ResourceType = resourceType;
            AccessLevel = accessLevel;
            GrantedAtUtc = DateTime.UtcNow;
            GrantedByUserId = grantedByUserId;
        }

        // Simple update method
        public void UpdateAccessLevel(AccessLevel newAccessLevel)
        {
            AccessLevel = newAccessLevel;
        }
    }
}