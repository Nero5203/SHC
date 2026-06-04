using Domain.Entities.StorageNodes;

namespace application.Ports.Driving.StorageNodes
{
    public interface IGetStorageNodeByIdUseCase
    {
        Task<StorageNode?> ExecuteAsync(Guid storageNodeId);
    }
}
