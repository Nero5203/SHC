using System;
using System.Diagnostics.Contracts;
using Domain.Entities.Roles;
using Domain.Entities.Users;
using SHC.Domain.Entities.Permissions.Enums;

namespace SHC.Domain.Entities.Permissions
{
    public class AuditLog
    {
        public Guid AuditLogId { get; private set; }
        public DateTime TimestampUtc { get; private set; }

        // Who/What performed the action?
        public Guid? SubjectId { get; private set; }
        public SubjectType SubjectType { get; private set; }

        // Categorization
        public string Action { get; private set; } = string.Empty;
        public ResourceType ResourceType { get; private set; }
        public string ResourceId { get; private set; } = string.Empty;
        // Outcome
        public bool IsSuccess { get; private set; }

        // Flexible payload
        public string PayloadJson { get; private set; } = string.Empty;
    }
}