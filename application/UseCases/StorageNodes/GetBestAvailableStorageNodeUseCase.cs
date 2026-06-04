using Domain.Entities.StorageNodes;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.StorageNodes;

namespace application.UseCases.StorageNodes
{
    public class GetBestAvailableStorageNodeUseCase : IGetBestAvailableStorageNodeUseCase
    {
        private readonly IStorageNodeRepository _storageNodeRepository;

        public GetBestAvailableStorageNodeUseCase(IStorageNodeRepository storageNodeRepository)
        {
            _storageNodeRepository = storageNodeRepository;
        }

        public async Task<StorageNode?> ExecuteAsync(long requiredBytes)
        {
            return await _storageNodeRepository.GetBestAvailableNodeAsync(requiredBytes);
        }
    }
}
