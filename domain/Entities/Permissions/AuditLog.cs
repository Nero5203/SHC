using System;
using System.Diagnostics.Contracts;
using Domain.Entities.Roles;
using Domain.Entities.Users;
using SHC.Domain.Entities.Permissions.Enums;

namespace SHC.Domain.Entities.Permissions
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public DateTime TimestampUtc { get; set; }

        // Who/What performed the action?
        public Guid? SubjectId { get; set; }
        public SubjectType SubjectType { get; set; }

        // Categorization
        public string Action { get; set; } = string.Empty;
        public ResourceType ResourceType { get; set; }
        public string ResourceId { get; set; } = string.Empty;
        // Outcome
        public bool IsSuccess { get; set; }

        // Flexible payload
        public string PayloadJson { get; set; } = string.Empty;
    }
}
