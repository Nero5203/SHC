using application.Ports.Driven.Trash;
using application.Ports.Driving.Trash;
using Domain.Entities.Trash;

namespace application.UseCases.Trash
{
    public class RestoreTrashedItemUseCase : IRestoreTrashedItemUseCase
    {
        private readonly ITrashRepository _trashRepository;

        public RestoreTrashedItemUseCase(ITrashRepository trashRepository)
        {
            _trashRepository = trashRepository;
        }

        public async Task<TrashedItem?> ExecuteAsync(Guid trashedItemId)
        {
            var trashedItem = await _trashRepository.GetByIdAsync(trashedItemId);

            if (trashedItem == null)
            {
                return null;
            }

            trashedItem.RestoredAt = DateTime.UtcNow;

            await _trashRepository.UpdateAsync(trashedItem);

            return trashedItem;
        }
    }
}
