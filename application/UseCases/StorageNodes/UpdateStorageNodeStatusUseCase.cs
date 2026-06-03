using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;
using ports.DrivenPorts.StorageNodes;
using ports.DrivingPorts.StorageNodes;

namespace application.UseCases.StorageNodes
{
    public class UpdateStorageNodeStatusUseCase : IUpdateStorageNodeStatusUseCase
    {
        private readonly IStorageNodeRepository _storageNodeRepository;

        public UpdateStorageNodeStatusUseCase(IStorageNodeRepository storageNodeRepository)
        {
            _storageNodeRepository = storageNodeRepository;
        }

        public async Task<StorageNode?> ExecuteAsync(Guid storageNodeId, NodeStatus status)
        {
            var storageNode = await _storageNodeRepository.GetByIdAsync(storageNodeId);

            if (storageNode == null)
            {
                return null;
            }

            await _storageNodeRepository.UpdateStatusAsync(storageNode, status);

            return storageNode;
        }
    }
}
