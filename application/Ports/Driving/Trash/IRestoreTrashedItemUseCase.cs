using Domain.Entities.Trash;

namespace application.Ports.Driving.Trash
{
    public interface IRestoreTrashedItemUseCase
    {
        Task<TrashedItem?> ExecuteAsync(Guid trashedItemId);
    }
}
