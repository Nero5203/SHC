namespace application.Ports.Driving
{
    public interface IDeleteUserUseCase
    {
        Task<bool> ExecuteAsync(Guid userId);
    }
}
