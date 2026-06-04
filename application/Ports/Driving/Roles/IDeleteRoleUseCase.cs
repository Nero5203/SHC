namespace application.Ports.Driving.Roles
{
    public interface IDeleteRoleUseCase
    {
        Task<bool> ExecuteAsync(Guid roleId);
    }
}
