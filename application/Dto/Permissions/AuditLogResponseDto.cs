using SHC.Domain.Entities.Permissions.Enums;

namespace application.Dto.Permissions
{
    public class AuditLogResponseDto
    {
        public Guid AuditLogId { get; set; }
        public DateTime TimestampUtc { get; set; }
        public Guid? SubjectId { get; set; }
        public SubjectType SubjectType { get; set; }
        public string Action { get; set; } = string.Empty;
        public ResourceType ResourceType { get; set; }
        public string ResourceId { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string PayloadJson { get; set; } = string.Empty;
    }
}
