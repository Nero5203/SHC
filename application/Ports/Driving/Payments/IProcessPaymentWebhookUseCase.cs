namespace application.Ports.Driving.Payments
{
    public interface IProcessPaymentWebhookUseCase
    {
        Task<bool> ExecuteAsync(string payload, string signatureHeader);
    }
}
