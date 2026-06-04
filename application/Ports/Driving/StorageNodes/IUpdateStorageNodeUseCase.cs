using Domain.Entities.StorageNodes;

namespace application.Ports.Driving.StorageNodes
{
    public interface IUpdateStorageNodeUseCase
    {
        Task<StorageNode?> ExecuteAsync(
            Guid storageNodeId,
            string name,
            string hostname,
            string ipAddress,
            int port,
            string basePath,
            long totalCapacityBytes);
    }
}
