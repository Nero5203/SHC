using Domain.Entities.Users;
using Domain.Entities.StorageNodes;
using Domain.Entities.LinkSharing;




namespace Domain.Entities.FileStorage
{ 

public class FileItem
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; }
    public string Url { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public Guid? FolderId { get; set; } 
    public Folder? Folder { get; set; } 

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

   public Guid StorageNodeId { get; set; }
   public StorageNode StorageNode { get; set; } = null!;


        public ICollection<SharedLink> SharedLinks { get; set; } = new List<SharedLink>();
}
}