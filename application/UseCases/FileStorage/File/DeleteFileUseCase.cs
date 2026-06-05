using application.Ports.Driven.FileStorage;
using application.Ports.Driven.Trash;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.Trash;

namespace application.UseCases.FileStorage.File
{
    public class DeleteFileUseCase : IDeleteFileUseCase
    {
        private readonly IFileRepository _fileRepository;
        private readonly ITrashRepository _trashRepository;

        public DeleteFileUseCase(
            IFileRepository fileRepository,
            ITrashRepository trashRepository)
        {
            _fileRepository = fileRepository;
            _trashRepository = trashRepository;
        }

        public async Task<bool> ExecuteAsync(Guid fileItemId)
        {
            var fileItem = await _fileRepository.GetByIdAsync(fileItemId);

            if (fileItem == null || fileItem.IsDeleted)
            {
                return false;
            }

            var now = DateTime.UtcNow;

            fileItem.IsDeleted = true;
            fileItem.UpdatedAt = now;

            await _fileRepository.UpdateAsync(fileItem);

            await _trashRepository.CreateAsync(new TrashedItem
            {
                TrashedItemId = Guid.NewGuid(),
                UserId = fileItem.UserId,
                OriginalItemId = fileItem.FileItemId,
                ItemType = "File",
                Name = fileItem.FileName,
                OriginalPath = fileItem.Url,
                OriginalParentId = fileItem.FolderId,
                Size = fileItem.FileSize,
                DeletedAt = now,
                ExpiresAt = now.AddDays(30)
            });

            return true;
        }
    }
}
