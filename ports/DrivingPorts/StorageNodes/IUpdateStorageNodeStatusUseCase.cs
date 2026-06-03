using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;

namespace ports.DrivingPorts.StorageNodes
{
    public interface IUpdateStorageNodeStatusUseCase
    {
        Task<StorageNode?> ExecuteAsync(Guid storageNodeId, NodeStatus status);
    }
}
