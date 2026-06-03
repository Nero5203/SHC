using Domain.Entities.StorageNodes;

namespace ports.DrivingPorts.StorageNodes
{
    public interface ICreateStorageNodeUseCase
    {
        Task<StorageNode> ExecuteAsync(
            string name,
            string hostname,
            string ipAddress,
            int port,
            string basePath,
            long totalCapacityBytes);
    }
}
