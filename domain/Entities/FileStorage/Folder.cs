using Domain.Entities.User;

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
    public ICollection<File> Files { get; set; } = new List<File>();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}