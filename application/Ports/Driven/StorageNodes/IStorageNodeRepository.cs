using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;

namespace application.Ports.Driven.StorageNodes
{
    public interface IStorageNodeRepository
    {
        Task CreateAsync(StorageNode storageNode);
        Task<StorageNode?> GetByIdAsync(Guid storageNodeId);
        Task<IReadOnlyList<StorageNode>> GetAllAsync();
        Task<StorageNode?> GetBestAvailableNodeAsync(long requiredBytes);
        Task UpdateAsync(StorageNode storageNode);
        Task UpdateHeartbeatAsync(
            StorageNode storageNode,
            long totalCapacityBytes,
            long usedCapacityBytes,
            NodeStatus status);
        Task UpdateStatusAsync(StorageNode storageNode, NodeStatus status);
    }
}
