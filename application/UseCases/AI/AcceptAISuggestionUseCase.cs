using application.Ports.Driven.AI;
using application.Ports.Driving.AI;
using Domain.Entities.AI;
using Domain.Entities.AI.Enums;

namespace application.UseCases.AI
{
    public class AcceptAISuggestionUseCase : IAcceptAISuggestionUseCase
    {
        private readonly IAISuggestionRepository _aiSuggestionRepository;

        public AcceptAISuggestionUseCase(IAISuggestionRepository aiSuggestionRepository)
        {
            _aiSuggestionRepository = aiSuggestionRepository;
        }

        public async Task<AISuggestion?> ExecuteAsync(Guid userId, Guid suggestionId)
        {
            var suggestion = await _aiSuggestionRepository.GetByIdAsync(suggestionId);
            if (suggestion == null || suggestion.UserId != userId)
            {
                return null;
            }

            suggestion.Status = AISuggestionStatus.Accepted;
            suggestion.AcceptedAt = DateTime.UtcNow;
            suggestion.DismissedAt = null;

            await _aiSuggestionRepository.UpdateAsync(suggestion);
            return suggestion;
        }
    }
}
