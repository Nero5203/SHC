namespace application.Ports.Driving.Roles
{
    public interface IRemoveRoleFromUserUseCase
    {
        Task<bool> ExecuteAsync(Guid roleId, Guid userId);
    }
}
