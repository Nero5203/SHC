using application.Common.Authorization;
using application.Ports.Driving.Auth;

namespace api.Authorization
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly ICurrentUserService _currentUserService;

        public UserAuthorizationService(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public bool HasRole(string role)
        {
            return _currentUserService.Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        public bool HasPermission(string permission)
        {
            return _currentUserService.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }

        public bool IsAdmin()
        {
            return HasRole(AuthorizationRoles.Admin) || HasPermission(AuthorizationPermissions.SystemAdmin);
        }
    }
}
