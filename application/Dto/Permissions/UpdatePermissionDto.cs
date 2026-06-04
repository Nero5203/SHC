using SHC.Domain.Entities.Permissions.Enums;

namespace application.Dto.Permissions
{
    public class UpdatePermissionDto
    {
        public AccessLevel AccessLevel { get; set; }
        public Guid? GrantedBySubjectId { get; set; }
        public SubjectType? GrantedBySubjectType { get; set; }
    }
}
