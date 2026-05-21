
using Domain.Entities.LinkSharing;
using Domain.Entities.Users;


namespace Domain.Entities.FileStorage
{ 
public class Folder
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
    public Guid? ParentFolderId { get; set; }
    public Folder? ParentFolder { get; set; }
    public ICollection<Folder> SubFolders { get; set; } = new List<Folder>();
    public ICollection<FileItem> Files { get; set; } = new List<FileItem>();
    public ICollection<SharedLink> SharedLinks { get; set; } = new List<SharedLink>();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
   



    }
}