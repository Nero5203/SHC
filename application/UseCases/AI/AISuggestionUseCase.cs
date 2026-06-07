using application.Ports.Driven.AI;
using application.Ports.Driven.FileStorage;
using Domain.Entities.AI;
using Domain.Entities.AI.Enums;

namespace application.UseCases.AI
{
    public class AISuggestionUseCase
    {
        private readonly IFileActivityRepository _fileActivityRepository;
        private readonly IAISuggestionRepository _aiSuggestionRepository;

        public AISuggestionUseCase(IFileActivityRepository fileActivityRepository, IAISuggestionRepository aiSuggestionRepository)
        {
            _fileActivityRepository = fileActivityRepository;
            _aiSuggestionRepository = aiSuggestionRepository;
        }
            public async Task ExecuteAsync(Guid userId)
        {
            var recentActivities = await _fileActivityRepository.GetRecentByUserAsync(userId, 10);
            var list = new List<AISuggestion>();

            foreach (var activity in recentActivities)
            {
                var exists = await _aiSuggestionRepository.ExistsAsync(userId, activity.FileItemId, AISuggestionType.ContinueWorking);
                if (exists) continue;

                list.Add(new AISuggestion
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FileItemId = activity.FileItemId,
                    Type = AISuggestionType.ContinueWorking,
                    Status = AISuggestionStatus.Pending,
                    Title = "Continue working on this file?",
                    Description = "We noticed you recently worked on this file. Would you like to continue working on it or need any assistance?",
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _aiSuggestionRepository.AddRangeAsync(list);
        }
    }   
}