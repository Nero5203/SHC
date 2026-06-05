using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.File
{
    public interface IGetFileByIdUseCase
    {
        Task<FileItem?> ExecuteAsync(Guid fileItemId);
    }
}
