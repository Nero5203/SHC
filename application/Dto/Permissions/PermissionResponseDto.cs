using SHC.Domain.Entities.Permissions.Enums;

namespace application.Dto.Permissions
{
    public class PermissionResponseDto
    {
        public Guid PermissionId { get; set; }
        public Guid SubjectId { get; set; }
        public SubjectType SubjectType { get; set; }
        public Guid ResourceId { get; set; }
        public ResourceType ResourceType { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public DateTime GrantedAtUtc { get; set; }
        public Guid? GrantedBySubjectId { get; set; }
        public SubjectType GrantedBySubjectType { get; set; }
    }
}
