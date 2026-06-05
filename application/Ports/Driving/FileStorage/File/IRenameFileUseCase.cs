using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.File
{
    public interface IRenameFileUseCase
    {
        Task<FileItem?> ExecuteAsync(Guid fileItemId, string newName);
    }
}
