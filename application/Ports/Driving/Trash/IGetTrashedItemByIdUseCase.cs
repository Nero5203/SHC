using Domain.Entities.Trash;

namespace application.Ports.Driving.Trash
{
    public interface IGetTrashedItemByIdUseCase
    {
        Task<TrashedItem?> ExecuteAsync(Guid trashedItemId);
    }
}
