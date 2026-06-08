using Domain.Entities.FileStorage;

namespace application.Ports.Driven.FileStorage
{
    public interface IFileRepository
    {
        Task CreateAsync(FileItem fileItem);
        Task<FileItem?> GetByIdAsync(Guid fileItemId);
        Task<IReadOnlyList<FileItem>> GetByFolderIdAsync(Guid userId, Guid? folderId);
        Task<IReadOnlyList<FileItem>> SearchAsync(Guid userId, string? query, Guid? folderId);
        Task UpdateAsync(FileItem fileItem);
        Task DeleteAsync(FileItem fileItem);
    }
}
