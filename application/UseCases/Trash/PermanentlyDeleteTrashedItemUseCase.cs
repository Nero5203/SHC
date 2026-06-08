using application.Ports.Driven.Trash;
using application.Ports.Driven.FileStorage;
using application.Ports.Driving.Trash;

namespace application.UseCases.Trash
{
    public class PermanentlyDeleteTrashedItemUseCase : IPermanentlyDeleteTrashedItemUseCase
    {
        private readonly ITrashRepository _trashRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IFileStorageRepository _fileStorageRepository;

        public PermanentlyDeleteTrashedItemUseCase(
            ITrashRepository trashRepository,
            IFileRepository fileRepository,
            IFolderRepository folderRepository,
            IFileStorageRepository fileStorageRepository)
        {
            _trashRepository = trashRepository;
            _fileRepository = fileRepository;
            _folderRepository = folderRepository;
            _fileStorageRepository = fileStorageRepository;
        }

        public async Task<bool> ExecuteAsync(Guid trashedItemId)
        {
            var trashedItem = await _trashRepository.GetByIdAsync(trashedItemId);

            if (trashedItem == null)
            {
                return false;
            }

            if (string.Equals(trashedItem.ItemType, "File", StringComparison.OrdinalIgnoreCase))
            {
                var fileItem = await _fileRepository.GetByIdAsync(trashedItem.OriginalItemId);

                if (fileItem != null)
                {
                    await _fileStorageRepository.DeleteAsync(fileItem.StorageNode, fileItem.Url);
                    await _fileRepository.DeleteAsync(fileItem);
                }
            }
            else if (string.Equals(trashedItem.ItemType, "Folder", StringComparison.OrdinalIgnoreCase))
            {
                var folder = await _folderRepository.GetByIdAsync(trashedItem.OriginalItemId);

                if (folder != null)
                {
                    var fileItems = await _folderRepository.GetTreeFileItemsAsync(folder.FolderId);

                    foreach (var fileItem in fileItems)
                    {
                        await _fileStorageRepository.DeleteAsync(fileItem.StorageNode, fileItem.Url);
                    }

                    await _folderRepository.HardDeleteTreeAsync(folder);
                }
            }

            await _trashRepository.DeleteAsync(trashedItem);

            return true;
        }
    }
}
