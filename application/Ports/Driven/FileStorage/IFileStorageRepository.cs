using Domain.Entities.StorageNodes;

namespace application.Ports.Driven.FileStorage
{
    public interface IFileStorageRepository
    {
        Task<string> SaveAsync(
            StorageNode storageNode,
            Guid fileItemId,
            string fileName,
            Stream content,
            CancellationToken cancellationToken = default);

        Task<Stream?> OpenReadAsync(
            StorageNode storageNode,
            string storedPath,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            StorageNode storageNode,
            string storedPath,
            CancellationToken cancellationToken = default);
    }
}
