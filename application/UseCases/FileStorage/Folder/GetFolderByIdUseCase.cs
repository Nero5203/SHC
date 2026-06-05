using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.Folder;

namespace application.UseCases.FileStorage.Folder
{
    public class GetFolderByIdUseCase : IGetFolderByIdUseCase
    {
        private readonly IFolderRepository _folderRepository;

        public GetFolderByIdUseCase(IFolderRepository folderRepository)
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

            return folder;
        }
    }
}
