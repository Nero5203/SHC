using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.Folder;
using Domain.Entities.FileStorage;

namespace application.UseCases.FileStorage.Folder
{
    public class ListFolderContentUseCase : IListFolderContentUseCase
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IFileRepository _fileRepository;

        public ListFolderContentUseCase(
            IFolderRepository folderRepository,
            IFileRepository fileRepository)
        {
            _folderRepository = folderRepository;
            _fileRepository = fileRepository;
        }

        public async Task<(IReadOnlyList<Domain.Entities.FileStorage.Folder> Folders, IReadOnlyList<FileItem> Files)?> ExecuteAsync(
            Guid userId,
            Guid? folderId)
        {
            if (folderId.HasValue)
            {
                var folder = await _folderRepository.GetByIdAsync(folderId.Value);

                if (folder == null || folder.IsDeleted || folder.UserId != userId)
                {
                    return null;
                }
            }

            var folders = await _folderRepository.GetByParentFolderIdAsync(userId, folderId);
            var files = await _fileRepository.GetByFolderIdAsync(userId, folderId);

            return (folders, files);
        }
    }
}
