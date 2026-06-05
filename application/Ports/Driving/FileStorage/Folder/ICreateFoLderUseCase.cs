namespace application.Ports.Driving.FileStorage.Folder
{
    public interface ICreateFolderUseCase
    {
        Task<Domain.Entities.FileStorage.Folder> ExecuteAsync(
            Guid userId,
            string name,
            Guid? parentFolderId);
    }
}
