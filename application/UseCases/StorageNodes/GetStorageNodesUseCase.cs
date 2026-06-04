using Domain.Entities.StorageNodes;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.StorageNodes;

namespace application.UseCases.StorageNodes
{
    public class GetStorageNodesUseCase : IGetStorageNodesUseCase
    {
        private readonly IStorageNodeRepository _storageNodeRepository;

        public GetStorageNodesUseCase(IStorageNodeRepository storageNodeRepository)
        {
            _storageNodeRepository = storageNodeRepository;
        }

        public async Task<IReadOnlyList<StorageNode>> ExecuteAsync()
        {
            return await _storageNodeRepository.GetAllAsync();
        }
    }
}
