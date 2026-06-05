using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.Folder
{
    public interface IRenameFolderUseCase
    {
        Task<Domain.Entities.FileStorage.Folder?> ExecuteAsync(Guid folderId, string newName);
    }
}
