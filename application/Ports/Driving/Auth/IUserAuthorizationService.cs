namespace application.Ports.Driving.Auth
{
    public interface IUserAuthorizationService
    {
        bool HasRole(string role);
        bool HasPermission(string permission);
        bool IsAdmin();
    }
}
