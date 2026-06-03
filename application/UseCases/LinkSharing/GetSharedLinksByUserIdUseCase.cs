using Domain.Entities.LinkSharing;
using ports.DrivenPorts.LinkSharing;
using ports.DrivingPorts.LinkSharing;

namespace application.UseCases.LinkSharing
{
    public class GetSharedLinksByUserIdUseCase : IGetSharedLinksByUserIdUseCase
    {
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public GetSharedLinksByUserIdUseCase(ISharedLinkRepository sharedLinkRepository)
        {
            _sharedLinkRepository = sharedLinkRepository;
        }

        public async Task<IReadOnlyList<SharedLink>> ExecuteAsync(Guid userId)
        {
            return await _sharedLinkRepository.GetByUserIdAsync(userId);
        }
    }
}
