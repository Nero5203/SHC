using Domain.Entities.User;

public class File
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
}