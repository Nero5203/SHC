using application.Dto.Subscriptions;

namespace application.Ports.Driving.Subscriptions
{
    public interface IGetSubscriptionEntitlementsUseCase
    {
        Task<SubscriptionEntitlementResponseDto> ExecuteAsync(Guid userId);
    }
}
