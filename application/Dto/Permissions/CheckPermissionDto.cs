using SHC.Domain.Entities.Permissions.Enums;

namespace application.Dto.Permissions
{
    public class CheckPermissionDto
    {
        public Guid SubjectId { get; set; }
        public SubjectType SubjectType { get; set; }
        public Guid ResourceId { get; set; }
        public ResourceType ResourceType { get; set; }
        public AccessLevel RequiredAccessLevel { get; set; }
    }
}
