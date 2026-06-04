using SHC.Domain.Entities.Permissions.Enums;

namespace application.Dto.Permissions
{
    public class CheckPermissionResponseDto
    {
        public bool HasAccess { get; set; }
        public AccessLevel RequiredAccessLevel { get; set; }
        public AccessLevel? GrantedAccessLevel { get; set; }
    }
}
