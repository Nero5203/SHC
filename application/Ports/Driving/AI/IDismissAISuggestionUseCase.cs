using Domain.Entities.AI;

namespace application.Ports.Driving.AI
{
    public interface IDismissAISuggestionUseCase
    {
        Task<AISuggestion?> ExecuteAsync(Guid userId, Guid suggestionId);
    }
}
