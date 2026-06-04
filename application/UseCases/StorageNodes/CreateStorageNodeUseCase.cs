using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.StorageNodes;

namespace application.UseCases.StorageNodes
{
    public class CreateStorageNodeUseCase : ICreateStorageNodeUseCase
    {
        private readonly IStorageNodeRepository _storageNodeRepository;

        public CreateStorageNodeUseCase(IStorageNodeRepository storageNodeRepository)
        {
            _storageNodeRepository = storageNodeRepository;
        }

        public async Task<StorageNode> ExecuteAsync(
            string name,
            string hostname,
            string ipAddress,
            int port,
            string basePath,
            long totalCapacityBytes)
        {
            var storageNode = new StorageNode
            {
                StorageNodeId = Guid.NewGuid(),
                Name = name,
                Hostname = hostname,
                IpAddress = ipAddress,
                Port = port,
                BasePath = basePath,
                TotalCapacityBytes = totalCapacityBytes,
                UsedCapacityBytes = 0,
                Status = NodeStatus.Offline,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _storageNodeRepository.CreateAsync(storageNode);

            return storageNode;
        }
    }
}
