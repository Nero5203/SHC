namespace Domain.Entities.Trash
{
    public class TrashedItem
    {
        public Guid TrashedItemId { get; set; }

        //owner of the trashed item
        public Guid UserId { get; set; }

        //original reference
        public Guid OriginalItemId { get; set; }
        public string ItemType { get; set; } = null!;

        //snapshot data
        public string Name { get; set; } = null!;
        public string? OriginalPath { get; set; }
        public Guid? OriginalParentId { get; set; }
        public long? Size { get; set; }

        //deletion metadata
        public DateTime DeletedAt { get; set; }

        //restore metadata
        public DateTime? RestoredAt { get; set; }

        //lifecycle
        public DateTime ExpiresAt { get; set; }
    }
}