namespace application.Dto.Roles
{
    public class RoleResponseDto
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserCount { get; set; }
    }
}
