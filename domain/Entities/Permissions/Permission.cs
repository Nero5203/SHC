using System;
using SHC.Domain.Entities.Permissions.Enums;

namespace SHC.Domain.Entities.Permissions
{
    public class Permission
    {
        public Guid PermissionId { get; set; }

        // The Actor
        public Guid SubjectId { get; set; }
        public SubjectType SubjectType { get; set; }

        // The Target
        public Guid ResourceId { get; set; }
        public ResourceType ResourceType { get; set; }

        // The Capability
        public AccessLevel AccessLevel { get; set; }

        // Metadata
        public DateTime GrantedAtUtc { get; set; }
        public Guid? GrantedBySubjectId { get; set; }
        public SubjectType GrantedBySubjectType { get; set; }

    }
}
