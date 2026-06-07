using Domain.Entities.AI;
using Domain.Entities.AI.Enums;

namespace application.Ports.Driven.AI
{
    public interface IAISuggestionRepository
    {
        Task AddRangeAsync(List<AISuggestion> suggestions);
        Task<bool> ExistsAsync(Guid userId, Guid fileItemId, AISuggestionType suggestionType);
    }
}