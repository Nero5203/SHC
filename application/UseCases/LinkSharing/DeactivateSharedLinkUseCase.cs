using Domain.Entities.LinkSharing;
using application.Ports.Driven.LinkSharing;
using application.Ports.Driving.LinkSharing;

namespace application.UseCases.LinkSharing
{
    public class DeactivateSharedLinkUseCase : IDeactivateSharedLinkUseCase
    {
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public DeactivateSharedLinkUseCase(ISharedLinkRepository sharedLinkRepository)
        {
            _sharedLinkRepository = sharedLinkRepository;
        }

        public async Task<SharedLink?> ExecuteAsync(Guid sharedLinkId)
        {
            var sharedLink = await _sharedLinkRepository.GetByIdAsync(sharedLinkId);

            if (sharedLink == null)
            {
                return null;
            }

            sharedLink.IsActive = false;
            sharedLink.UpdatedAt = DateTime.UtcNow;

            await _sharedLinkRepository.UpdateAsync(sharedLink);

            return sharedLink;
        }
    }
}
