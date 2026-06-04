using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.StorageNodes;

namespace application.UseCases.StorageNodes
{
    public class UpdateStorageNodeHeartbeatUseCase : IUpdateStorageNodeHeartbeatUseCase
    {
        private readonly IStorageNodeRepository _storageNodeRepository;

        public UpdateStorageNodeHeartbeatUseCase(IStorageNodeRepository storageNodeRepository)
        {
            _storageNodeRepository = storageNodeRepository;
        }

        public async Task<StorageNode?> ExecuteAsync(
            Guid storageNodeId,
            long totalCapacityBytes,
            long usedCapacityBytes,
            NodeStatus status)
        {
            var storageNode = await _storageNodeRepository.GetByIdAsync(storageNodeId);

            if (storageNode == null)
            {
                return null;
            }

            await _storageNodeRepository.UpdateHeartbeatAsync(
                storageNode,
                totalCapacityBytes,
                usedCapacityBytes,
                status);

            return storageNode;
        }
    }
}
