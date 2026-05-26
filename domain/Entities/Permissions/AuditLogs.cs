using System;
using SHC.Domain.Entities.Permissions.Enums;

namespace SHC.Domain.Entities.Permissions
{
    public class AuditLog
    {
        public Guid Id { get; private set; }
        public DateTime TimestampUtc { get; private set; }

        // Who/What performed the action?
        public Guid? SubjectId { get; private set; }
        public SubjectType SubjectType { get; private set; }

        // Categorization
        public string Action { get; private set; } = string.Empty;
        public ResourceType ResourceType { get; private set; }
        public string ResourceId { get; private set; } = string.Empty;

        // Contextual network data
        public string IpAddress { get; private set; } = string.Empty;
        public string UserAgent { get; private set; } = string.Empty;

        // Outcome
        public bool IsSuccess { get; private set; }

        // Flexible payload
        public string PayloadJson { get; private set; } = string.Empty;

        // EF Core Constructor
        private AuditLog() { }

        // Master Constructor
        public AuditLog(
            Guid? subjectId,
            SubjectType subjectType,
            string action,
            ResourceType resourceType,
            string resourceId,
            string ipAddress,
            string userAgent,
            bool isSuccess,
            string payloadJson = "{}")
        {
            Id = Guid.NewGuid();
            TimestampUtc = DateTime.UtcNow;
            SubjectId = subjectId;
            SubjectType = subjectType;
            Action = action;
            ResourceType = resourceType;
            ResourceId = resourceId;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            IsSuccess = isSuccess;
            PayloadJson = payloadJson;
        }
    }
}