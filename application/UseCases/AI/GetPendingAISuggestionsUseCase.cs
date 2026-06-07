using application.Ports.Driven.AI;
using application.Ports.Driving.AI;
using Domain.Entities.AI;

namespace application.UseCases.AI
{
    public class GetPendingAISuggestionsUseCase : IGetPendingAISuggestionsUseCase
    {
        private readonly IAISuggestionRepository _aiSuggestionRepository;

        public GetPendingAISuggestionsUseCase(IAISuggestionRepository aiSuggestionRepository)
        {
            _aiSuggestionRepository = aiSuggestionRepository;
        }

        public async Task<IReadOnlyList<AISuggestion>> ExecuteAsync(Guid userId)
        {
            return await _aiSuggestionRepository.GetPendingByUserAsync(userId);
        }
    }
}
