using application.Ports.Driven.Trash;
using application.Ports.Driving.Trash;
using Domain.Entities.Trash;

namespace application.UseCases.Trash
{
    public class GetTrashedItemsByUserIdUseCase : IGetTrashedItemsByUserIdUseCase
    {
        private readonly ITrashRepository _trashRepository;

        public GetTrashedItemsByUserIdUseCase(ITrashRepository trashRepository)
        {
            _trashRepository = trashRepository;
        }

        public async Task<IReadOnlyList<TrashedItem>> ExecuteAsync(Guid userId)
        {
            return await _trashRepository.GetByUserIdAsync(userId);
        }
    }
}
