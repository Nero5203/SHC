using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.FileStorage;

namespace application.UseCases.FileStorage.File
{
    public class GetFileByIdUseCase : IGetFileByIdUseCase
    {
        private readonly IFileRepository _fileRepository;

        public GetFileByIdUseCase(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<FileItem?> ExecuteAsync(Guid fileItemId)
        {
            var fileItem = await _fileRepository.GetByIdAsync(fileItemId);

            if (fileItem == null || fileItem.IsDeleted)
            {
                return null;
            }

            return fileItem;
        }
    }
}
