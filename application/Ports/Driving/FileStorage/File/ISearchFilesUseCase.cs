using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.File
{
    public interface ISearchFilesUseCase
    {
        Task<IReadOnlyList<FileItem>> ExecuteAsync(Guid userId, string? query, Guid? folderId);
    }
}
