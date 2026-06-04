using Domain.Entities.StorageNodes;

namespace application.Ports.Driving.StorageNodes
{
    public interface IGetBestAvailableStorageNodeUseCase
    {
        Task<StorageNode?> ExecuteAsync(long requiredBytes);
    }
}
