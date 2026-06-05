using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.FileStorage;

namespace application.UseCases.FileStorage.File
{
    public class MoveFileUseCase : IMoveFileUseCase
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFolderRepository _folderRepository;

        public MoveFileUseCase(
            IFileRepository fileRepository,
            IFolderRepository folderRepository)
        {
            _fileRepository = fileRepository;
            _folderRepository = folderRepository;
        }

        public async Task<FileItem?> ExecuteAsync(Guid fileItemId, Guid? targetFolderId)
        {
            var fileItem = await _fileRepository.GetByIdAsync(fileItemId);

            if (fileItem == null || fileItem.IsDeleted)
            {
                return null;
            }

            if (targetFolderId.HasValue)
            {
                var targetFolder = await _folderRepository.GetByIdAsync(targetFolderId.Value);

                if (targetFolder == null || targetFolder.IsDeleted || targetFolder.UserId != fileItem.UserId)
                {
                    throw new InvalidOperationException("Target folder was not found for this user.");
                }
            }

            fileItem.FolderId = targetFolderId;
            fileItem.UpdatedAt = DateTime.UtcNow;

            await _fileRepository.UpdateAsync(fileItem);

            return fileItem;
        }
    }
}
