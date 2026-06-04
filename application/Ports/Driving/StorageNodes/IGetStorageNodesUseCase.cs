using Domain.Entities.StorageNodes;

namespace application.Ports.Driving.StorageNodes
{
    public interface IGetStorageNodesUseCase
    {
        Task<IReadOnlyList<StorageNode>> ExecuteAsync();
    }
}
