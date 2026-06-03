using Domain.Entities.StorageNodes;

namespace ports.DrivingPorts.StorageNodes
{
    public interface IGetStorageNodeByIdUseCase
    {
        Task<StorageNode?> ExecuteAsync(Guid storageNodeId);
    }
}
