using Domain.Entities.LinkSharing;
using ports.DrivenPorts.LinkSharing;
using ports.DrivingPorts.LinkSharing;

namespace application.UseCases.LinkSharing
{
    public class UpdateSharedLinkUseCase : IUpdateSharedLinkUseCase
    {
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public UpdateSharedLinkUseCase(ISharedLinkRepository sharedLinkRepository)
        {
            _sharedLinkRepository = sharedLinkRepository;
        }

        public async Task<SharedLink?> ExecuteAsync(
            Guid sharedLinkId,
            DateTime? expirationDate,
            bool? isActive,
            bool? canView,
            bool? canEdit,
            bool? allowDownload)
        {
            var sharedLink = await _sharedLinkRepository.GetByIdAsync(sharedLinkId);

            if (sharedLink == null)
            {
                return null;
            }

            sharedLink.ExpirationDate = expirationDate;

            if (isActive.HasValue)
            {
                sharedLink.IsActive = isActive.Value;
            }

            if (canView.HasValue)
            {
                sharedLink.CanView = canView.Value;
            }

            if (canEdit.HasValue)
            {
                sharedLink.CanEdit = canEdit.Value;
            }

            if (allowDownload.HasValue)
            {
                sharedLink.AllowDownload = allowDownload.Value;
            }

            sharedLink.UpdatedAt = DateTime.UtcNow;

            await _sharedLinkRepository.UpdateAsync(sharedLink);

            return sharedLink;
        }
    }
}
