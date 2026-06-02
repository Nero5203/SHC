namespace api.Dto.AI
{
    public class AIFileInsightResponseDto
    {
        public Guid Id { get; set; }
        public Guid FileItemId { get; set; }
        public string? Summary { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public string? DetectedLanguage { get; set; }
        public bool IsSensitive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
