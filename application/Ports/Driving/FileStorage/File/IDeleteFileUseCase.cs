namespace application.Ports.Driving.FileStorage.File
{
    public interface IDeleteFileUseCase
    {
        Task<bool> ExecuteAsync(Guid fileItemId);
    }
}
