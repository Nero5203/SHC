namespace application.Ports.Driving.Trash
{
    public interface IPermanentlyDeleteTrashedItemUseCase
    {
        Task<bool> ExecuteAsync(Guid trashedItemId);
    }
}
