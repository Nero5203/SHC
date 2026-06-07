using domain.Entities.FileStorage.Enums;

namespace domain.Entities.FileStorage
{
    public class FileActivity
    {
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid FileItemId { get; set; }

    public FileActivityType Type { get; set; }

    public DateTime CreatedAt { get; set; }    }
}