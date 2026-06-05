using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.Folder;

namespace application.UseCases.FileStorage.Folder
{
    public class CreateFolderUseCase : ICreateFolderUseCase
    {
        private readonly IFolderRepository _folderRepository;

        public CreateFolderUseCase(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        public async Task<Domain.Entities.FileStorage.Folder> ExecuteAsync(
            Guid userId,
            string name,
            Guid? parentFolderId)
        {
            if (parentFolderId.HasValue)
            {
                var parentFolder = await _folderRepository.GetByIdAsync(parentFolderId.Value);

                if (parentFolder == null || parentFolder.IsDeleted || parentFolder.UserId != userId)
                {
                    throw new InvalidOperationException("Parent folder was not found for this user.");
                }
            }

            var now = DateTime.UtcNow;
            var folder = new Domain.Entities.FileStorage.Folder
            {
                FolderId = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                ParentFolderId = parentFolderId,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            await _folderRepository.CreateAsync(folder);

            return folder;
        }
    }
}
