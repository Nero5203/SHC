using Domain.Entities.FileStorage;

namespace application.Ports.Driving.FileStorage.File
{
    public interface IDownloadFileUseCase
    {
        Task<(FileItem FileItem, Stream Content)?> ExecuteAsync(Guid fileItemId);
    }
}
