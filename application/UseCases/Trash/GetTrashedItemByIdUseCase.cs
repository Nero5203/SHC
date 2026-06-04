using application.Ports.Driven.Trash;
using application.Ports.Driving.Trash;
using Domain.Entities.Trash;

namespace application.UseCases.Trash
{
    public class GetTrashedItemByIdUseCase : IGetTrashedItemByIdUseCase
    {
        private readonly ITrashRepository _trashRepository;

        public GetTrashedItemByIdUseCase(ITrashRepository trashRepository)
        {
            _trashRepository = trashRepository;
        }

        public async Task<TrashedItem?> ExecuteAsync(Guid trashedItemId)
        {
            return await _trashRepository.GetByIdAsync(trashedItemId);
        }
    }
}
