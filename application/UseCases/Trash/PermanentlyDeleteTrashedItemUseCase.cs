using application.Ports.Driven.Trash;
using application.Ports.Driving.Trash;

namespace application.UseCases.Trash
{
    public class PermanentlyDeleteTrashedItemUseCase : IPermanentlyDeleteTrashedItemUseCase
    {
        private readonly ITrashRepository _trashRepository;

        public PermanentlyDeleteTrashedItemUseCase(ITrashRepository trashRepository)
        {
            _trashRepository = trashRepository;
        }

        public async Task<bool> ExecuteAsync(Guid trashedItemId)
        {
            var trashedItem = await _trashRepository.GetByIdAsync(trashedItemId);

            if (trashedItem == null)
            {
                return false;
            }

            await _trashRepository.DeleteAsync(trashedItem);

            return true;
        }
    }
}
