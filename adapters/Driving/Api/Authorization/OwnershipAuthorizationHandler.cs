using System.Security.Claims;
using application.Common.Authorization;
using application.Ports.Driven.FileStorage;
using application.Ports.Driving.Permissions;
using Microsoft.AspNetCore.Authorization;
using SHC.Domain.Entities.Permissions.Enums;

namespace api.Authorization
{
    public sealed class OwnershipRequirement : IAuthorizationRequirement
    {
        public OwnershipRequirement(ResourceType resourceType, AccessLevel requiredAccessLevel)
        {
            ResourceType = resourceType;
            RequiredAccessLevel = requiredAccessLevel;
        }

        public ResourceType ResourceType { get; }
        public AccessLevel RequiredAccessLevel { get; }
    }

    public class OwnershipAuthorizationHandler : AuthorizationHandler<OwnershipRequirement, Guid>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly ICheckPermissionUseCase _checkPermissionUseCase;

        public OwnershipAuthorizationHandler(
            IFileRepository fileRepository,
            IFolderRepository folderRepository,
            ICheckPermissionUseCase checkPermissionUseCase)
        {
            _fileRepository = fileRepository;
            _folderRepository = folderRepository;
            _checkPermissionUseCase = checkPermissionUseCase;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnershipRequirement requirement,
            Guid resourceId)
        {
            if (IsAdmin(context))
            {
                context.Succeed(requirement);
                return;
            }

            var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return;
            }

            if (await IsOwnerAsync(userId, resourceId, requirement.ResourceType))
            {
                context.Succeed(requirement);
                return;
            }

            var sharedAccess = await _checkPermissionUseCase.ExecuteAsync(
                userId,
                SubjectType.User,
                resourceId,
                requirement.ResourceType,
                requirement.RequiredAccessLevel);

            if (sharedAccess.HasAccess)
            {
                context.Succeed(requirement);
            }
        }

        private async Task<bool> IsOwnerAsync(Guid userId, Guid resourceId, ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.File => (await _fileRepository.GetByIdAsync(resourceId))?.UserId == userId,
                ResourceType.Folder => (await _folderRepository.GetByIdAsync(resourceId))?.UserId == userId,
                _ => false
            };
        }

        private static bool IsAdmin(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole(AuthorizationRoles.Admin) ||
                context.User.FindAll(ShcClaimTypes.Permission)
                    .Any(claim => string.Equals(claim.Value, AuthorizationPermissions.SystemAdmin, StringComparison.OrdinalIgnoreCase));
        }
    }
}
