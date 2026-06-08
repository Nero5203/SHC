using adapters.Driven.Persistence.Data;
using application.Ports.Driven.FileStorage;
using Domain.Entities.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.FileStorage
{
    public class FolderRepository : IFolderRepository
    {
        private readonly ShcDbContext _context;

        public FolderRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Folder folder)
        {
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
        }

        public async Task<Folder?> GetByIdAsync(Guid folderId)
        {
            return await _context.Folders
                .Include(f => f.FileItems)
                .Include(f => f.SubFolders)
                .FirstOrDefaultAsync(f => f.FolderId == folderId);
        }

        public async Task<IReadOnlyList<Folder>> GetByParentFolderIdAsync(Guid userId, Guid? parentFolderId)
        {
            return await _context.Folders
                .Where(f =>
                    f.UserId == userId &&
                    f.ParentFolderId == parentFolderId &&
                    !f.IsDeleted)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<bool> IsDescendantAsync(Guid folderId, Guid possibleDescendantId)
        {
            var current = await _context.Folders
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FolderId == possibleDescendantId);

            while (current != null)
            {
                if (current.ParentFolderId == folderId)
                {
                    return true;
                }

                if (!current.ParentFolderId.HasValue)
                {
                    return false;
                }

                current = await _context.Folders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.FolderId == current.ParentFolderId.Value);
            }

            return false;
        }

        public async Task UpdateAsync(Folder folder)
        {
            _context.Folders.Update(folder);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteTreeAsync(Folder folder)
        {
            await SoftDeleteFolderAsync(folder.FolderId, DateTime.UtcNow);
            await _context.SaveChangesAsync();
        }

        public async Task RestoreTreeAsync(Folder folder)
        {
            await RestoreFolderAsync(folder.FolderId, DateTime.UtcNow);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<FileItem>> GetTreeFileItemsAsync(Guid folderId)
        {
            var folderIds = new List<Guid>();
            await CollectFolderIdsAsync(folderId, folderIds);

            return await _context.FileItems
                .Include(fi => fi.StorageNode)
                .Where(fi => fi.FolderId.HasValue && folderIds.Contains(fi.FolderId.Value))
                .ToListAsync();
        }

        public async Task HardDeleteTreeAsync(Folder folder)
        {
            await HardDeleteFolderAsync(folder.FolderId);
            await _context.SaveChangesAsync();
        }

        private async Task SoftDeleteFolderAsync(Guid folderId, DateTime deletedAt)
        {
            var folder = await _context.Folders
                .Include(f => f.FileItems)
                .FirstOrDefaultAsync(f => f.FolderId == folderId);

            if (folder == null)
            {
                return;
            }

            folder.IsDeleted = true;
            folder.UpdatedAt = deletedAt;

            foreach (var fileItem in folder.FileItems)
            {
                fileItem.IsDeleted = true;
                fileItem.UpdatedAt = deletedAt;
            }

            var childFolderIds = await _context.Folders
                .Where(f => f.ParentFolderId == folderId && !f.IsDeleted)
                .Select(f => f.FolderId)
                .ToListAsync();

            foreach (var childFolderId in childFolderIds)
            {
                await SoftDeleteFolderAsync(childFolderId, deletedAt);
            }
        }

        private async Task RestoreFolderAsync(Guid folderId, DateTime restoredAt)
        {
            var folder = await _context.Folders
                .Include(f => f.FileItems)
                .FirstOrDefaultAsync(f => f.FolderId == folderId);

            if (folder == null)
            {
                return;
            }

            folder.IsDeleted = false;
            folder.UpdatedAt = restoredAt;

            foreach (var fileItem in folder.FileItems)
            {
                fileItem.IsDeleted = false;
                fileItem.UpdatedAt = restoredAt;
            }

            var childFolderIds = await _context.Folders
                .Where(f => f.ParentFolderId == folderId)
                .Select(f => f.FolderId)
                .ToListAsync();

            foreach (var childFolderId in childFolderIds)
            {
                await RestoreFolderAsync(childFolderId, restoredAt);
            }
        }

        private async Task CollectFolderIdsAsync(Guid folderId, List<Guid> folderIds)
        {
            folderIds.Add(folderId);

            var childFolderIds = await _context.Folders
                .Where(f => f.ParentFolderId == folderId)
                .Select(f => f.FolderId)
                .ToListAsync();

            foreach (var childFolderId in childFolderIds)
            {
                await CollectFolderIdsAsync(childFolderId, folderIds);
            }
        }

        private async Task HardDeleteFolderAsync(Guid folderId)
        {
            var childFolders = await _context.Folders
                .Where(f => f.ParentFolderId == folderId)
                .ToListAsync();

            foreach (var childFolder in childFolders)
            {
                await HardDeleteFolderAsync(childFolder.FolderId);
            }

            var files = await _context.FileItems
                .Where(fi => fi.FolderId == folderId)
                .ToListAsync();

            _context.FileItems.RemoveRange(files);

            var folder = await _context.Folders
                .FirstOrDefaultAsync(f => f.FolderId == folderId);

            if (folder != null)
            {
                _context.Folders.Remove(folder);
            }
        }
    }
}
