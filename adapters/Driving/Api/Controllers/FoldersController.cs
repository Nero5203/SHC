using api.Authorization;
using application.Common.Authorization;
using application.Dto.FileStorage.File;
using application.Dto.FileStorage.Folder;
using application.Ports.Driving.FileStorage.Folder;
using application.Ports.Driving.Notifications;
using Domain.Entities.FileStorage;
using Domain.Entities.LinkSharing;
using Domain.Entities.Notifications.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHC.Domain.Entities.Permissions.Enums;

namespace api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/folders")]
    public class FoldersController : ControllerBase
    {
        private readonly ICreateFolderUseCase _createFolderUseCase;
        private readonly IGetFolderByIdUseCase _getFolderByIdUseCase;
        private readonly IListFolderContentUseCase _listFolderContentUseCase;
        private readonly IRenameFolderUseCase _renameFolderUseCase;
        private readonly IMoveFolderUseCase _moveFolderUseCase;
        private readonly IDeleteFolderUseCase _deleteFolderUseCase;
        private readonly IArchiveFolderUseCase _archiveFolderUseCase;
        private readonly IShareFolderUseCase _shareFolderUseCase;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICreateNotificationUseCase _createNotificationUseCase;

        public FoldersController(
            ICreateFolderUseCase createFolderUseCase,
            IGetFolderByIdUseCase getFolderByIdUseCase,
            IListFolderContentUseCase listFolderContentUseCase,
            IRenameFolderUseCase renameFolderUseCase,
            IMoveFolderUseCase moveFolderUseCase,
            IDeleteFolderUseCase deleteFolderUseCase,
            IArchiveFolderUseCase archiveFolderUseCase,
            IShareFolderUseCase shareFolderUseCase,
            IAuthorizationService authorizationService,
            ICreateNotificationUseCase createNotificationUseCase)
        {
            _createFolderUseCase = createFolderUseCase;
            _getFolderByIdUseCase = getFolderByIdUseCase;
            _listFolderContentUseCase = listFolderContentUseCase;
            _renameFolderUseCase = renameFolderUseCase;
            _moveFolderUseCase = moveFolderUseCase;
            _deleteFolderUseCase = deleteFolderUseCase;
            _archiveFolderUseCase = archiveFolderUseCase;
            _shareFolderUseCase = shareFolderUseCase;
            _authorizationService = authorizationService;
            _createNotificationUseCase = createNotificationUseCase;
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.FolderManage)]
        public async Task<ActionResult<CreateFolderResponseDto>> CreateFolder(CreateFolderRequestDto dto)
        {
            try
            {
                var folder = await _createFolderUseCase.ExecuteAsync(
                    dto.UserId,
                    dto.Name,
                    dto.ParentFolderId);

                await CreateFolderNotificationAsync(
                    "Folder created",
                    $"You created the folder {folder.Name}.",
                    folder.UserId,
                    folder.FolderId,
                    NotificationType.FileUpdated);

                return CreatedAtAction(
                    nameof(GetFolderById),
                    new { folderId = folder.FolderId },
                    new CreateFolderResponseDto { Folder = MapFolder(folder) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{folderId:guid}")]
        [Authorize(Policy = AuthorizationPolicies.FileRead)]
        public async Task<ActionResult<FolderDto>> GetFolderById(Guid folderId)
        {
            var folder = await _getFolderByIdUseCase.ExecuteAsync(folderId);

            if (folder == null)
            {
                return NotFound();
            }

            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                folderId,
                new OwnershipRequirement(ResourceType.Folder, AccessLevel.Read));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            return Ok(MapFolder(folder));
        }

        [HttpGet("user/{userId:guid}/contents")]
        public async Task<ActionResult<ListFolderContentsResponseDto>> ListFolderContents(
            Guid userId,
            [FromQuery] Guid? folderId)
        {
            var contents = await _listFolderContentUseCase.ExecuteAsync(userId, folderId);

            if (contents == null)
            {
                return NotFound();
            }

            return Ok(new ListFolderContentsResponseDto
            {
                Folders = contents.Value.Folders.Select(MapFolder).ToList(),
                Files = contents.Value.Files.Select(MapFile).ToList()
            });
        }

        [HttpPut("{folderId:guid}/rename")]
        [Authorize(Policy = AuthorizationPolicies.FolderManage)]
        public async Task<ActionResult<RenameFolderResponseDto>> RenameFolder(
            Guid folderId,
            RenameFolderRequestDto dto)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                folderId,
                new OwnershipRequirement(ResourceType.Folder, AccessLevel.Write));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var folder = await _renameFolderUseCase.ExecuteAsync(folderId, dto.NewName);

            if (folder == null)
            {
                return NotFound();
            }

            await CreateFolderNotificationAsync(
                "Folder renamed",
                $"Your folder was renamed to {folder.Name}.",
                folder.UserId,
                folder.FolderId,
                NotificationType.FileUpdated);

            return Ok(new RenameFolderResponseDto { Folder = MapFolder(folder) });
        }

        [HttpPut("{folderId:guid}/move")]
        [Authorize(Policy = AuthorizationPolicies.FolderManage)]
        public async Task<ActionResult<MoveFolderResponseDto>> MoveFolder(
            Guid folderId,
            MoveFolderRequestDto dto)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                folderId,
                new OwnershipRequirement(ResourceType.Folder, AccessLevel.Write));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            try
            {
                var folder = await _moveFolderUseCase.ExecuteAsync(folderId, dto.TargetParentFolderId);

                if (folder == null)
                {
                    return NotFound();
                }

                await CreateFolderNotificationAsync(
                    "Folder moved",
                    $"{folder.Name} was moved.",
                    folder.UserId,
                    folder.FolderId,
                    NotificationType.FileUpdated);

                return Ok(new MoveFolderResponseDto { Folder = MapFolder(folder) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{folderId:guid}/archive")]
        [Authorize(Policy = AuthorizationPolicies.FolderManage)]
        public async Task<ActionResult<FolderDto>> ArchiveFolder(Guid folderId)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                folderId,
                new OwnershipRequirement(ResourceType.Folder, AccessLevel.Write));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var folder = await _archiveFolderUseCase.ExecuteAsync(folderId);

            if (folder == null)
            {
                return NotFound();
            }

            await CreateFolderNotificationAsync(
                "Folder archived",
                $"{folder.Name} was archived.",
                folder.UserId,
                folder.FolderId,
                NotificationType.FileUpdated);

            return Ok(MapFolder(folder));
        }

        [HttpDelete("{folderId:guid}")]
        [Authorize(Policy = AuthorizationPolicies.FolderManage)]
        public async Task<ActionResult<DeleteFolderResponseDto>> DeleteFolder(Guid folderId)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                folderId,
                new OwnershipRequirement(ResourceType.Folder, AccessLevel.Delete));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var deleted = await _deleteFolderUseCase.ExecuteAsync(folderId);

            if (!deleted)
            {
                return NotFound();
            }

            await CreateFolderNotificationAsync(
                "Folder deleted",
                "A folder was moved to trash.",
                User.GetUserId(),
                folderId,
                NotificationType.FileUpdated);

            return Ok(new DeleteFolderResponseDto { Success = true });
        }

        [HttpPost("{folderId:guid}/share")]
        [Authorize(Policy = AuthorizationPolicies.FolderManage)]
        public async Task<ActionResult<ShareFolderResponseDto>> ShareFolder(
            Guid folderId,
            ShareFolderRequestDto dto)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                folderId,
                new OwnershipRequirement(ResourceType.Folder, AccessLevel.Admin));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var sharedLink = await _shareFolderUseCase.ExecuteAsync(
                folderId,
                dto.Permission,
                dto.ExpiresAt);

            if (sharedLink == null)
            {
                return NotFound();
            }

            await CreateFolderNotificationAsync(
                "Folder shared",
                "A share link was created for one of your folders.",
                sharedLink.UserId,
                folderId,
                NotificationType.FolderShared);

            return Ok(MapShareFolder(sharedLink));
        }

        private async Task CreateFolderNotificationAsync(
            string title,
            string message,
            Guid userId,
            Guid folderId,
            NotificationType type)
        {
            await _createNotificationUseCase.ExecuteAsync(
                title,
                message,
                type,
                NotificationChannel.InApp,
                userId,
                User.GetUserId(),
                null,
                folderId);
        }

        private static FolderDto MapFolder(Domain.Entities.FileStorage.Folder folder)
        {
            return new FolderDto
            {
                FolderId = folder.FolderId,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId,
                UserId = folder.UserId,
                CreatedAt = folder.CreatedAt,
                UpdatedAt = folder.UpdatedAt,
                IsDeleted = folder.IsDeleted
            };
        }

        private static FileDto MapFile(FileItem fileItem)
        {
            return new FileDto
            {
                FileItemId = fileItem.FileItemId,
                FileName = fileItem.FileName,
                FileType = fileItem.FileType,
                FileSize = fileItem.FileSize,
                Url = fileItem.Url,
                FolderId = fileItem.FolderId,
                UserId = fileItem.UserId,
                StorageNodeId = fileItem.StorageNodeId,
                CreatedAt = fileItem.CreatedAt,
                UpdatedAt = fileItem.UpdatedAt,
                IsDeleted = fileItem.IsDeleted
            };
        }

        private static ShareFolderResponseDto MapShareFolder(SharedLink sharedLink)
        {
            return new ShareFolderResponseDto
            {
                ShareLinkId = sharedLink.SharedLinkId,
                ShareUrl = $"/api/shared-links/token/{sharedLink.TokenUrl}",
                ExpiresAt = sharedLink.ExpirationDate
            };
        }
    }
}
