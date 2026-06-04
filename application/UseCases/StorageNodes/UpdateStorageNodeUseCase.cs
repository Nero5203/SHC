using Domain.Entities.StorageNodes;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.StorageNodes;

namespace application.UseCases.StorageNodes
{
    public class UpdateStorageNodeUseCase : IUpdateStorageNodeUseCase
    {
        private readonly IStorageNodeRepository _storageNodeRepository;

        public UpdateStorageNodeUseCase(IStorageNodeRepository storageNodeRepository)
        {
            _storageNodeRepository = storageNodeRepository;
        }

        public async Task<StorageNode?> ExecuteAsync(
            Guid storageNodeId,
            string name,
            string hostname,
            string ipAddress,
            int port,
            string basePath,
            long totalCapacityBytes)
        {
            var storageNode = await _storageNodeRepository.GetByIdAsync(storageNodeId);

            if (storageNode == null)
            {
                return null;
            }

            storageNode.Name = name;
            storageNode.Hostname = hostname;
            storageNode.IpAddress = ipAddress;
            storageNode.Port = port;
            storageNode.BasePath = basePath;
            storageNode.TotalCapacityBytes = totalCapacityBytes;
            storageNode.UpdatedAt = DateTime.UtcNow;

            await _storageNodeRepository.UpdateAsync(storageNode);

            return storageNode;
        }
    }
}
