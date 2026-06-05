using Domain.Entities.StorageNodes;

namespace application.Ports.Driven.FileStorage
{
    public interface IStorageNodeAllocator
    {
        Task<StorageNode?> GetBestAvailableNodeAsync(long requiredBytes);
    }
}
