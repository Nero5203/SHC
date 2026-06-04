using Domain.Entities.StorageNodes;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.StorageNodes;

namespace application.UseCases.StorageNodes
{
    public class GetStorageNodeByIdUseCase : IGetStorageNodeByIdUseCase
    {
        private readonly IStorageNodeRepository _storageNodeRepository;

        public GetStorageNodeByIdUseCase(IStorageNodeRepository storageNodeRepository)
        {
            _storageNodeRepository = storageNodeRepository;
        }

        public async Task<StorageNode?> ExecuteAsync(Guid storageNodeId)
        {
            return await _storageNodeRepository.GetByIdAsync(storageNodeId);
        }
    }
}
