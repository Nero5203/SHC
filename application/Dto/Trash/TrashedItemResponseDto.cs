namespace application.Dto.Trash
{
    public class TrashedItemResponseDto
    {
        public Guid TrashedItemId { get; set; }
        public Guid UserId { get; set; }
        public Guid OriginalItemId { get; set; }
        public string ItemType { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? OriginalPath { get; set; }
        public Guid? OriginalParentId { get; set; }
        public long? Size { get; set; }
        public DateTime DeletedAt { get; set; }
        public DateTime? RestoredAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
