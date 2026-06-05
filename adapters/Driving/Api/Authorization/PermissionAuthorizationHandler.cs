using application.Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace api.Authorization
{
    public sealed class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(params string[] permissions)
        {
            Permissions = permissions;
        }

        public IReadOnlyCollection<string> Permissions { get; }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var permissionClaims = context.User
                .FindAll(ShcClaimTypes.Permission)
                .Select(claim => claim.Value)
                .ToList();

            var hasPermission = requirement.Permissions
                .Any(permission => permissionClaims.Contains(permission, StringComparer.OrdinalIgnoreCase));

            var isAdmin = permissionClaims.Contains(AuthorizationPermissions.SystemAdmin, StringComparer.OrdinalIgnoreCase);

            if (hasPermission || isAdmin)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
