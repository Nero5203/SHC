using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.Folder;

namespace application.UseCases.FileStorage.Folder
{
    public class MoveFolderUseCase : IMoveFolderUseCase
    {
        private readonly IFolderRepository _folderRepository;

        public MoveFolderUseCase(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        public async Task<Domain.Entities.FileStorage.Folder?> ExecuteAsync(Guid folderId, Guid? targetParentFolderId)
        {
            var folder = await _folderRepository.GetByIdAsync(folderId);

            if (folder == null || folder.IsDeleted)
            {
                return null;
            }

            if (targetParentFolderId == folderId)
            {
                throw new InvalidOperationException("Folder cannot be moved inside itself.");
            }

            if (targetParentFolderId.HasValue)
            {
                var targetParentFolder = await _folderRepository.GetByIdAsync(targetParentFolderId.Value);

                if (targetParentFolder == null ||
                    targetParentFolder.IsDeleted ||
                    targetParentFolder.UserId != folder.UserId)
                {
                    throw new InvalidOperationException("Target parent folder was not found for this user.");
                }

                var targetIsDescendant = await _folderRepository.IsDescendantAsync(folderId, targetParentFolderId.Value);

                if (targetIsDescendant)
                {
                    throw new InvalidOperationException("Folder cannot be moved inside one of its own subfolders.");
                }
            }

            folder.ParentFolderId = targetParentFolderId;
            folder.UpdatedAt = DateTime.UtcNow;

            await _folderRepository.UpdateAsync(folder);

            return folder;
        }
    }
}
