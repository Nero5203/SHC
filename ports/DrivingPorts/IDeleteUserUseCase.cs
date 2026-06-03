namespace ports.DrivingPorts
{
    public interface IDeleteUserUseCase
    {
        Task<bool> ExecuteAsync(Guid userId);
    }
}
