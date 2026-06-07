using application.Ports.Driven.AI;
using application.Ports.Driven.FileStorage;
using application.Ports.Driving.AI;
using Domain.Entities.AI;
using Domain.Entities.AI.Enums;
using domain.Entities.FileStorage.Enums;

namespace application.UseCases.AI
{
    public class AISuggestionUseCase : IGenerateAISuggestionsUseCase
    {
        private readonly IFileActivityRepository _fileActivityRepository;
        private readonly IAISuggestionRepository _aiSuggestionRepository;
        private readonly IFileRepository _fileRepository;

        public AISuggestionUseCase(
            IFileActivityRepository fileActivityRepository,
            IAISuggestionRepository aiSuggestionRepository,
            IFileRepository fileRepository)
        {
            _fileActivityRepository = fileActivityRepository;
            _aiSuggestionRepository = aiSuggestionRepository;
            _fileRepository = fileRepository;
        }

        public async Task<IReadOnlyList<AISuggestion>> ExecuteAsync(Guid userId)
        {
            var recentActivities = await _fileActivityRepository.GetRecentByUserAsync(userId, 10);
            var processedFileIds = new HashSet<Guid>();
            var list = new List<AISuggestion>();

            foreach (var activity in recentActivities)
            {
                if (activity.Type == FileActivityType.Deleted)
                {
                    continue;
                }

                if (!processedFileIds.Add(activity.FileItemId))
                {
                    continue;
                }

                var fileItem = await _fileRepository.GetByIdAsync(activity.FileItemId);
                if (fileItem == null || fileItem.IsDeleted || fileItem.UserId != userId)
                {
                    continue;
                }

                var exists = await _aiSuggestionRepository.ExistsAsync(userId, activity.FileItemId, AISuggestionType.ContinueWorking);
                if (exists)
                {
                    continue;
                }

                list.Add(new AISuggestion
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FileItemId = activity.FileItemId,
                    Type = AISuggestionType.ContinueWorking,
                    Status = AISuggestionStatus.Pending,
                    Title = "Continue working on this file?",
                    Description = "We noticed you recently worked on this file. Would you like to continue working on it or need any assistance?",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7)
                });
            }

            await _aiSuggestionRepository.AddRangeAsync(list);

            return await _aiSuggestionRepository.GetPendingByUserAsync(userId);
        }
    }
}
