using Domain.Entities.FileStorage;

namespace application.Ports.Driven.FileStorage
{
    public interface IFolderRepository
    {
        Task CreateAsync(Folder folder);
        Task<Folder?> GetByIdAsync(Guid folderId);
        Task<IReadOnlyList<Folder>> GetByParentFolderIdAsync(Guid userId, Guid? parentFolderId);
        Task<bool> IsDescendantAsync(Guid folderId, Guid possibleDescendantId);
        Task UpdateAsync(Folder folder);
        Task SoftDeleteTreeAsync(Folder folder);
        Task RestoreTreeAsync(Folder folder);
        Task<IReadOnlyList<FileItem>> GetTreeFileItemsAsync(Guid folderId);
        Task HardDeleteTreeAsync(Folder folder);
    }
}
