using Domain.Entities.AI.Enums;
using Domain.Entities.FileStorage;
using Domain.Entities.Users;

namespace Domain.Entities.AI
{
    public class AISuggestion
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid? FileItemId { get; set; }
        public FileItem? FileItem { get; set; }

        public AISuggestionType Type { get; set; }
        public AISuggestionStatus Status { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? SuggestedValue { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? DismissedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
