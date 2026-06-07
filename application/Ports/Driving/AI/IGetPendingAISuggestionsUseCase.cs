using Domain.Entities.AI;

namespace application.Ports.Driving.AI
{
    public interface IGetPendingAISuggestionsUseCase
    {
        Task<IReadOnlyList<AISuggestion>> ExecuteAsync(Guid userId);
    }
}
