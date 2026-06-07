using application.Ports.Driven.FileStorage;
using application.Ports.Driven.StorageNodes;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.FileStorage;
using System.Security.Cryptography;

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
            var (contentForSave, contentHash, disposeBufferedStream) = await PrepareContentAsync(content);

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
            try
            {
                var fileItem = new FileItem
                {
                    FileItemId = Guid.NewGuid(),
                    FileName = fileName,
                    FileType = string.IsNullOrWhiteSpace(fileType) ? "application/octet-stream" : fileType,
                    FileSize = fileSize,
                    FolderId = folderId,
                    UserId = userId,
                    StorageNodeId = storageNode.StorageNodeId,
                    ContentHash = contentHash,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                };

                fileItem.Url = await _fileStorageRepository.SaveAsync(
                    storageNode,
                    fileItem.FileItemId,
                    fileName,
                    contentForSave);

                storageNode.UsedCapacityBytes += fileSize;
                storageNode.UpdatedAt = now;

                await _fileRepository.CreateAsync(fileItem);
                await _storageNodeRepository.UpdateAsync(storageNode);

                return fileItem;
            }
            finally
            {
                if (disposeBufferedStream)
                {
                    await contentForSave.DisposeAsync();
                }
            }
        }

        private static async Task<(Stream Stream, string ContentHash, bool DisposeBufferedStream)> PrepareContentAsync(Stream content)
        {
            if (content.CanSeek)
            {
                if (content.Position != 0)
                {
                    content.Position = 0;
                }

                var contentHash = await ComputeSha256Async(content);
                content.Position = 0;

                return (content, contentHash, false);
            }

            var bufferedStream = new MemoryStream();
            await content.CopyToAsync(bufferedStream);
            bufferedStream.Position = 0;

            var bufferedHash = await ComputeSha256Async(bufferedStream);
            bufferedStream.Position = 0;

            return (bufferedStream, bufferedHash, true);
        }

        private static async Task<string> ComputeSha256Async(Stream content)
        {
            using var sha256 = SHA256.Create();
            var hash = await sha256.ComputeHashAsync(content);
            return Convert.ToHexString(hash);
        }
    }
}
