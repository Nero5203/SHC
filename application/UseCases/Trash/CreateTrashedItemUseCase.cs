using application.Ports.Driven.Trash;
using application.Ports.Driving.Trash;
using Domain.Entities.Trash;

namespace application.UseCases.Trash
{
    public class CreateTrashedItemUseCase : ICreateTrashedItemUseCase
    {
        private readonly ITrashRepository _trashRepository;

        public CreateTrashedItemUseCase(ITrashRepository trashRepository)
        {
            _trashRepository = trashRepository;
        }

        public async Task<TrashedItem> ExecuteAsync(
            Guid userId,
            Guid originalItemId,
            string itemType,
            string name,
            string? originalPath,
            Guid? originalParentId,
            long? size,
            DateTime? expiresAt)
        {
            var now = DateTime.UtcNow;

            var trashedItem = new TrashedItem
            {
                TrashedItemId = Guid.NewGuid(),
                UserId = userId,
                OriginalItemId = originalItemId,
                ItemType = itemType,
                Name = name,
                OriginalPath = originalPath,
                OriginalParentId = originalParentId,
                Size = size,
                DeletedAt = now,
                ExpiresAt = expiresAt ?? now.AddDays(30)
            };

            await _trashRepository.CreateAsync(trashedItem);

            return trashedItem;
        }
    }
}
