using Domain.Entities.Trash;

namespace application.Ports.Driving.Trash
{
    public interface IGetTrashedItemsByUserIdUseCase
    {
        Task<IReadOnlyList<TrashedItem>> ExecuteAsync(Guid userId);
    }
}
