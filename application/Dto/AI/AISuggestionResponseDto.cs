using Domain.Entities.AI.Enums;

namespace application.Dto.AI
{
    public class AISuggestionResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? FileItemId { get; set; }
        public string? FileName { get; set; }
        public AISuggestionType Type { get; set; }
        public AISuggestionStatus Status { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? SuggestedValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? DismissedAt { get; set; }
    }
}
