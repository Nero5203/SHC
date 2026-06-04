using Domain.Entities.Trash;

namespace application.Ports.Driving.Trash
{
    public interface ICreateTrashedItemUseCase
    {
        Task<TrashedItem> ExecuteAsync(
            Guid userId,
            Guid originalItemId,
            string itemType,
            string name,
            string? originalPath,
            Guid? originalParentId,
            long? size,
            DateTime? expiresAt);
    }
}
