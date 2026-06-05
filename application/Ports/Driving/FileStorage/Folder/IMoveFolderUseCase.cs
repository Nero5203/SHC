using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.Folder
{
    public interface IMoveFolderUseCase
    {
        Task<Domain.Entities.FileStorage.Folder?> ExecuteAsync(Guid folderId, Guid? targetParentFolderId);
    }
}
