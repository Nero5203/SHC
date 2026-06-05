using application.Ports.Driven.FileStorage;
using application.Ports.Driven.Trash;
using application.Ports.Driving.FileStorage.Folder;
using Domain.Entities.Trash;

namespace application.UseCases.FileStorage.Folder
{
    public class DeleteFolderUseCase : IDeleteFolderUseCase
    {
        private readonly IFolderRepository _folderRepository;
        private readonly ITrashRepository _trashRepository;

        public DeleteFolderUseCase(
            IFolderRepository folderRepository,
            ITrashRepository trashRepository)
        {
            _folderRepository = folderRepository;
            _trashRepository = trashRepository;
        }

        public async Task<bool> ExecuteAsync(Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(folderId);

            if (folder == null || folder.IsDeleted)
            {
                return false;
            }

            var now = DateTime.UtcNow;

            await _folderRepository.SoftDeleteTreeAsync(folder);

            await _trashRepository.CreateAsync(new TrashedItem
            {
                TrashedItemId = Guid.NewGuid(),
                UserId = folder.UserId,
                OriginalItemId = folder.FolderId,
                ItemType = "Folder",
                Name = folder.Name,
                OriginalParentId = folder.ParentFolderId,
                DeletedAt = now,
                ExpiresAt = now.AddDays(30)
            });

            return true;
        }
    }
}
