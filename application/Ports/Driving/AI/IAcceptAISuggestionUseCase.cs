using Domain.Entities.AI;

namespace application.Ports.Driving.AI
{
    public interface IAcceptAISuggestionUseCase
    {
        Task<AISuggestion?> ExecuteAsync(Guid userId, Guid suggestionId);
    }
}
