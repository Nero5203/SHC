using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.Folder;

namespace application.UseCases.FileStorage.Folder
{
    public class ArchiveFolderUseCase : IArchiveFolderUseCase
    {
        private readonly IFolderRepository _folderRepository;

        public ArchiveFolderUseCase(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        public async Task<Domain.Entities.FileStorage.Folder?> ExecuteAsync(Guid folderId)
        {
            var folder = await _folderRepository.GetByIdAsync(folderId);

            if (folder == null || folder.IsDeleted)
            {
                return null;
            }

            await _folderRepository.SoftDeleteTreeAsync(folder);

            return folder;
        }
    }
}
