using application.Ports.Driven.Trash;
using application.Ports.Driven.FileStorage;
using application.Ports.Driving.Trash;
using Domain.Entities.Trash;

namespace application.UseCases.Trash
{
    public class RestoreTrashedItemUseCase : IRestoreTrashedItemUseCase
    {
        private readonly ITrashRepository _trashRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFolderRepository _folderRepository;

        public RestoreTrashedItemUseCase(
            ITrashRepository trashRepository,
            IFileRepository fileRepository,
            IFolderRepository folderRepository)
        {
            _trashRepository = trashRepository;
            _fileRepository = fileRepository;
            _folderRepository = folderRepository;
        }

        public async Task<TrashedItem?> ExecuteAsync(Guid trashedItemId)
        {
            var trashedItem = await _trashRepository.GetByIdAsync(trashedItemId);

            if (trashedItem == null)
            {
                return null;
            }

            if (trashedItem.RestoredAt != null)
            {
                return trashedItem;
            }

            var now = DateTime.UtcNow;

            if (string.Equals(trashedItem.ItemType, "File", StringComparison.OrdinalIgnoreCase))
            {
                var fileItem = await _fileRepository.GetByIdAsync(trashedItem.OriginalItemId);

                if (fileItem == null)
                {
                    return null;
                }

                fileItem.IsDeleted = false;
                fileItem.UpdatedAt = now;

                await _fileRepository.UpdateAsync(fileItem);
            }
            else if (string.Equals(trashedItem.ItemType, "Folder", StringComparison.OrdinalIgnoreCase))
            {
                var folder = await _folderRepository.GetByIdAsync(trashedItem.OriginalItemId);

                if (folder == null)
                {
                    return null;
                }

                await _folderRepository.RestoreTreeAsync(folder);
            }

            trashedItem.RestoredAt = now;

            await _trashRepository.UpdateAsync(trashedItem);

            return trashedItem;
        }
    }
}
