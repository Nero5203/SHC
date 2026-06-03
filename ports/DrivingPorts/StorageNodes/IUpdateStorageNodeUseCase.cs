using Domain.Entities.StorageNodes;

namespace ports.DrivingPorts.StorageNodes
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
