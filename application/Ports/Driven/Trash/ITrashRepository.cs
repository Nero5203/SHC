using Domain.Entities.Trash;

namespace application.Ports.Driven.Trash
{
    public interface ITrashRepository
    {
        Task CreateAsync(TrashedItem trashedItem);
        Task<TrashedItem?> GetByIdAsync(Guid trashedItemId);
        Task<IReadOnlyList<TrashedItem>> GetByUserIdAsync(Guid userId);
        Task UpdateAsync(TrashedItem trashedItem);
        Task DeleteAsync(TrashedItem trashedItem);
    }
}
