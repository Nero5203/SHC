using Domain.Entities.StorageNodes;
using ports.DrivenPorts.StorageNodes;
using ports.DrivingPorts.StorageNodes;

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
