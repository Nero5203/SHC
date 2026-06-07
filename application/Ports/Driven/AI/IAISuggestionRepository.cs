using Domain.Entities.AI;
using Domain.Entities.AI.Enums;

namespace application.Ports.Driven.AI
{
    public interface IAISuggestionRepository
    {
        Task AddRangeAsync(List<AISuggestion> suggestions);
        Task<bool> ExistsAsync(Guid userId, Guid fileItemId, AISuggestionType suggestionType);
        Task<IReadOnlyList<AISuggestion>> GetPendingByUserAsync(Guid userId);
        Task<AISuggestion?> GetByIdAsync(Guid suggestionId);
        Task UpdateAsync(AISuggestion suggestion);
    }
}
