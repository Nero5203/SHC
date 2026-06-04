using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;

namespace application.Ports.Driving.StorageNodes
{
    public interface IUpdateStorageNodeStatusUseCase
    {
        Task<StorageNode?> ExecuteAsync(Guid storageNodeId, NodeStatus status);
    }
}
