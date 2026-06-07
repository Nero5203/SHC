using Domain.Entities.AI;

namespace application.Ports.Driving.AI
{
    public interface IGenerateAISuggestionsUseCase
    {
        Task<IReadOnlyList<AISuggestion>> ExecuteAsync(Guid userId);
    }
}
