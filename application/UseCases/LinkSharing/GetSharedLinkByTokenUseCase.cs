using Domain.Entities.LinkSharing;
using ports.DrivenPorts.LinkSharing;
using ports.DrivingPorts.LinkSharing;

namespace application.UseCases.LinkSharing
{
    public class GetSharedLinkByTokenUseCase : IGetSharedLinkByTokenUseCase
    {
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public GetSharedLinkByTokenUseCase(ISharedLinkRepository sharedLinkRepository)
        {
            _sharedLinkRepository = sharedLinkRepository;
        }

        public async Task<SharedLink?> ExecuteAsync(string tokenUrl)
        {
            return await _sharedLinkRepository.GetByTokenUrlAsync(tokenUrl);
        }
    }
}
