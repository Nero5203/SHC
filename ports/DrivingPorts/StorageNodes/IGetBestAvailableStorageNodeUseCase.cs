using Domain.Entities.StorageNodes;

namespace ports.DrivingPorts.StorageNodes
{
    public interface IGetBestAvailableStorageNodeUseCase
    {
        Task<StorageNode?> ExecuteAsync(long requiredBytes);
    }
}
