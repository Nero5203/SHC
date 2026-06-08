using adapters.Driven.Persistence.Data;
using application.Ports.Driven.FileStorage;
using Domain.Entities.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.FileStorage
{
    public class FileRepository : IFileRepository
    {
        private readonly ShcDbContext _context;

        public FileRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(FileItem fileItem)
        {
            _context.FileItems.Add(fileItem);
            await _context.SaveChangesAsync();
        }

        public async Task<FileItem?> GetByIdAsync(Guid fileItemId)
        {
            return await _context.FileItems
                .Include(fi => fi.Folder)
                .Include(fi => fi.StorageNode)
                .FirstOrDefaultAsync(fi => fi.FileItemId == fileItemId);
        }

        public async Task<IReadOnlyList<FileItem>> GetByFolderIdAsync(Guid userId, Guid? folderId)
        {
            return await _context.FileItems
                .Include(fi => fi.StorageNode)
                .Where(fi =>
                    fi.UserId == userId &&
                    fi.FolderId == folderId &&
                    !fi.IsDeleted)
                .OrderBy(fi => fi.FileName)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<FileItem>> SearchAsync(Guid userId, string? query, Guid? folderId)
        {
            var files = _context.FileItems
                .Include(fi => fi.StorageNode)
                .Where(fi => fi.UserId == userId && !fi.IsDeleted);

            if (folderId.HasValue)
            {
                files = files.Where(fi => fi.FolderId == folderId);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim();
                files = files.Where(fi => fi.FileName.Contains(normalizedQuery));
            }

            return await files
                .OrderBy(fi => fi.FileName)
                .ToListAsync();
        }

        public async Task UpdateAsync(FileItem fileItem)
        {
            _context.FileItems.Update(fileItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(FileItem fileItem)
        {
            _context.FileItems.Remove(fileItem);
            await _context.SaveChangesAsync();
        }
    }
}
