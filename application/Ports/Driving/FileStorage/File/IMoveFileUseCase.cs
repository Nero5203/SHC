using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.File
{
    public interface IMoveFileUseCase
    {
        Task<FileItem?> ExecuteAsync(Guid fileItemId, Guid? targetFolderId);
    }
}
