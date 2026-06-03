using Domain.Entities.StorageNodes;

namespace ports.DrivingPorts.StorageNodes
{
    public interface IGetStorageNodesUseCase
    {
        Task<IReadOnlyList<StorageNode>> ExecuteAsync();
    }
}
