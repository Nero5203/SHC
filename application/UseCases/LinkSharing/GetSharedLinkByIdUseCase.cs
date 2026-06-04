using Domain.Entities.LinkSharing;
using application.Ports.Driven.LinkSharing;
using application.Ports.Driving.LinkSharing;

namespace application.UseCases.LinkSharing
{
    public class GetSharedLinkByIdUseCase : IGetSharedLinkByIdUseCase
    {
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public GetSharedLinkByIdUseCase(ISharedLinkRepository sharedLinkRepository)
        {
            _sharedLinkRepository = sharedLinkRepository;
        }

        public async Task<SharedLink?> ExecuteAsync(Guid sharedLinkId)
        {
            return await _sharedLinkRepository.GetByIdAsync(sharedLinkId);
        }
    }
}
