namespace application.Ports.Driving.FileStorage.Folder
{
    public interface IDeleteFolderUseCase
    {
        Task<bool> ExecuteAsync(Guid folderId);
    }
}
