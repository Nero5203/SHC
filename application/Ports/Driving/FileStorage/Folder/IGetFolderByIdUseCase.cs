namespace application.Ports.Driving.FileStorage.Folder
{
    public interface IGetFolderByIdUseCase
    {
        Task<Domain.Entities.FileStorage.Folder?> ExecuteAsync(Guid folderId);
    }
}
