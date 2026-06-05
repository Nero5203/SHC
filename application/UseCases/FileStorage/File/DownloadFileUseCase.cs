using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.FileStorage;

namespace application.UseCases.FileStorage.File
{
    public class DownloadFileUseCase : IDownloadFileUseCase
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorageRepository _fileStorageRepository;

        public DownloadFileUseCase(
            IFileRepository fileRepository,
            IFileStorageRepository fileStorageRepository)
        {
            _fileRepository = fileRepository;
            _fileStorageRepository = fileStorageRepository;
        }

        public async Task<(FileItem FileItem, Stream Content)?> ExecuteAsync(Guid fileItemId)
        {
            var fileItem = await _fileRepository.GetByIdAsync(fileItemId);

            if (fileItem == null || fileItem.IsDeleted)
            {
                return null;
            }

            var content = await _fileStorageRepository.OpenReadAsync(
                fileItem.StorageNode,
                fileItem.Url);

            if (content == null)
            {
                return null;
            }

            return (fileItem, content);
        }
    }
}
