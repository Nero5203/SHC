using Domain.Entities.Purchases;

namespace application.Ports.Driving.Purchases
{
    public interface IListInvoicesUseCase
    {
        Task<IReadOnlyList<Invoice>> ExecuteAsync();
    }
}
