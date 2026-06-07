using Domain.Entities.LinkSharing;
using application.Ports.Driven.LinkSharing;
using application.Ports.Driving.LinkSharing;

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
            var sharedLink = await _sharedLinkRepository.GetByTokenUrlAsync(tokenUrl);

            if (sharedLink == null || !sharedLink.IsActive)
            {
                return null;
            }

            if (sharedLink.ExpirationDate.HasValue && sharedLink.ExpirationDate.Value <= DateTime.UtcNow)
            {
                sharedLink.IsActive = false;
                sharedLink.UpdatedAt = DateTime.UtcNow;

                await _sharedLinkRepository.UpdateAsync(sharedLink);

                return null;
            }

            return sharedLink;
        }
    }
}
