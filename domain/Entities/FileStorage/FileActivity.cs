using domain.Entities.FileStorage.Enums;
using Domain.Entities.Users;
using Domain.Entities.FileStorage;

namespace domain.Entities.FileStorage
{
    public class FileActivity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid FileItemId { get; set; }
        public FileItem FileItem { get; set; } = null!;

        public FileActivityType Type { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
