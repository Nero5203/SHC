using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;

namespace application.Ports.Driving.StorageNodes
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
