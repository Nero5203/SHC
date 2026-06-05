using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.File
{
    public interface IUploadFileUseCase
    {
        Task<FileItem> ExecuteAsync(
            Guid userId,
            Guid? folderId,
            string fileName,
            string fileType,
            long fileSize,
            Stream content);
    }
}
