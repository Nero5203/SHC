namespace application.Dto.Roles
{
    public class UserRoleResponseDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}
