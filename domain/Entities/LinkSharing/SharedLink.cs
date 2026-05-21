using Domain.Entities.User;

public class SharedLink
{
    public Guid Id { get; set; }
    public string TokenUrl { get; set; } = null!;
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublic { get; set; } = true;
    public Guid? FileId { get; set; }
    public File? File { get; set; } = null!;

    public Guid? FolderId { get; set; }
    public Folder? Folder { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}