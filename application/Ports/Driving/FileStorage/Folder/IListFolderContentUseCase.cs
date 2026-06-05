using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.Folder
{
    public interface IListFolderContentUseCase
    {
        Task<(IReadOnlyList<Domain.Entities.FileStorage.Folder> Folders, IReadOnlyList<FileItem> Files)?> ExecuteAsync(
            Guid userId,
            Guid? folderId);
    }
}
