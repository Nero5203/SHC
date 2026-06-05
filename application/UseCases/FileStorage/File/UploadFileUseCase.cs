using application.Ports.Driven.FileStorage;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.FileStorage;

namespace application.UseCases.FileStorage.File
{
    public class UploadFileUseCase : IUploadFileUseCase
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IStorageNodeAllocator _storageNodeAllocator;
        private readonly IFileStorageRepository _fileStorageRepository;
        private readonly IStorageNodeRepository _storageNodeRepository;

        public UploadFileUseCase(
            IFileRepository fileRepository,
            IFolderRepository folderRepository,
            IStorageNodeAllocator storageNodeAllocator,
            IFileStorageRepository fileStorageRepository,
            IStorageNodeRepository storageNodeRepository)
        {
            _fileRepository = fileRepository;
            _folderRepository = folderRepository;
            _storageNodeAllocator = storageNodeAllocator;
            _fileStorageRepository = fileStorageRepository;
            _storageNodeRepository = storageNodeRepository;
        }

        public async Task<FileItem> ExecuteAsync(
            Guid userId,
            Guid? folderId,
            string fileName,
            string fileType,
            long fileSize,
            Stream content)
        {
            if (folderId.HasValue)
            {
                var folder = await _folderRepository.GetByIdAsync(folderId.Value);

                if (folder == null || folder.IsDeleted || folder.UserId != userId)
                {
                    throw new InvalidOperationException("Target folder was not found for this user.");
                }
            }

            var storageNode = await _storageNodeAllocator.GetBestAvailableNodeAsync(fileSize);

            if (storageNode == null)
            {
                throw new InvalidOperationException("No available storage node has enough capacity.");
            }

            var now = DateTime.UtcNow;
            var fileItem = new FileItem
            {
                FileItemId = Guid.NewGuid(),
                FileName = fileName,
                FileType = string.IsNullOrWhiteSpace(fileType) ? "application/octet-stream" : fileType,
                FileSize = fileSize,
                FolderId = folderId,
                UserId = userId,
                StorageNodeId = storageNode.StorageNodeId,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            fileItem.Url = await _fileStorageRepository.SaveAsync(
                storageNode,
                fileItem.FileItemId,
                fileName,
                content);

            storageNode.UsedCapacityBytes += fileSize;
            storageNode.UpdatedAt = now;

            await _fileRepository.CreateAsync(fileItem);
            await _storageNodeRepository.UpdateAsync(storageNode);

            return fileItem;
        }
    }
}
