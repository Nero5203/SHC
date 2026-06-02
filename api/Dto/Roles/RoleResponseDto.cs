namespace api.Dto.Roles
{
    public class RoleResponseDto
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
