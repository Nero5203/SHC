using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;

namespace ports.DrivingPorts.StorageNodes
{
    public interface IUpdateStorageNodeHeartbeatUseCase
    {
        Task<StorageNode?> ExecuteAsync(
            Guid storageNodeId,
            long totalCapacityBytes,
            long usedCapacityBytes,
            NodeStatus status);
    }
}
