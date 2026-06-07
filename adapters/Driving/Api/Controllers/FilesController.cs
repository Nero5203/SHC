using api.Requests.FileStorage;
using api.Authorization;
using application.Common.Authorization;
using application.Dto.FileStorage.File;
using application.Ports.Driving.FileStorage.File;
using application.Ports.Driving.Notifications;
using Domain.Entities.FileStorage;
using Domain.Entities.LinkSharing;
using Domain.Entities.Notifications.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHC.Domain.Entities.Permissions.Enums;
using application.Ports.Driven.FileStorage;
using domain.Entities.FileStorage.Enums;

namespace api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IUploadFileUseCase _uploadFileUseCase;
        private readonly IGetFileByIdUseCase _getFileByIdUseCase;
        private readonly ISearchFilesUseCase _searchFilesUseCase;
        private readonly IDownloadFileUseCase _downloadFileUseCase;
        private readonly IRenameFileUseCase _renameFileUseCase;
        private readonly IMoveFileUseCase _moveFileUseCase;
        private readonly IDeleteFileUseCase _deleteFileUseCase;
        private readonly IShareFileUseCase _shareFileUseCase;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFileActivityRepository _fileActivityRepository;
        private readonly ICreateNotificationUseCase _createNotificationUseCase;

        public FilesController(
            IUploadFileUseCase uploadFileUseCase,
            IGetFileByIdUseCase getFileByIdUseCase,
            ISearchFilesUseCase searchFilesUseCase,
            IDownloadFileUseCase downloadFileUseCase,
            IRenameFileUseCase renameFileUseCase,
            IMoveFileUseCase moveFileUseCase,
            IDeleteFileUseCase deleteFileUseCase,
            IShareFileUseCase shareFileUseCase,
            IAuthorizationService authorizationService,
            IFileActivityRepository fileActivityRepository,
            ICreateNotificationUseCase createNotificationUseCase)
        {
            _uploadFileUseCase = uploadFileUseCase;
            _getFileByIdUseCase = getFileByIdUseCase;
            _searchFilesUseCase = searchFilesUseCase;
            _downloadFileUseCase = downloadFileUseCase;
            _renameFileUseCase = renameFileUseCase;
            _moveFileUseCase = moveFileUseCase;
            _deleteFileUseCase = deleteFileUseCase;
            _shareFileUseCase = shareFileUseCase;
            _authorizationService = authorizationService;
            _fileActivityRepository = fileActivityRepository;
            _createNotificationUseCase = createNotificationUseCase;
        }

        [HttpPost("upload")]
        [Authorize(Policy = AuthorizationPolicies.FileUpload)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UploadFileResponseDto>> UploadFile([FromForm] UploadFileFormRequest request)
        {
            var file = request.File;

            if (file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            try
            {
                using var content = file.OpenReadStream();
                var fileItem = await _uploadFileUseCase.ExecuteAsync(
                    request.UserId,
                    request.FolderId,
                    file.FileName,
                    file.ContentType,
                    file.Length,
                    content);
                
                await _fileActivityRepository.TrackAsync(
                    request.UserId,
                    fileItem.FileItemId,
                    FileActivityType.Uploaded
                );
                await CreateFileNotificationAsync(
                    "File uploaded",
                    $"You uploaded {fileItem.FileName}.",
                    fileItem.UserId,
                    fileItem.FileItemId);

                return CreatedAtAction(
                    nameof(GetFileById),
                    new { fileItemId = fileItem.FileItemId },
                    new UploadFileResponseDto { File = MapFile(fileItem) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{fileItemId:guid}")]
        [Authorize(Policy = AuthorizationPolicies.FileRead)]
        public async Task<ActionResult<FileDto>> GetFileById(Guid fileItemId)
        {
            var fileItem = await _getFileByIdUseCase.ExecuteAsync(fileItemId);

            if (fileItem == null)
            {
                return NotFound();
            }

            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                fileItemId,
                new OwnershipRequirement(ResourceType.File, AccessLevel.Read));

            await _fileActivityRepository.TrackAsync(
                User.GetUserId(),
                fileItem.FileItemId,
                FileActivityType.Viewed
            );

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            return Ok(MapFile(fileItem));
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<FileDto>>> SearchFiles(
            Guid userId,
            [FromQuery] string? query,
            [FromQuery] Guid? folderId)
        {
            var files = await _searchFilesUseCase.ExecuteAsync(userId, query, folderId);

            return Ok(files.Select(MapFile).ToList());
        }

        [HttpGet("{fileItemId:guid}/download")]
        [Authorize(Policy = AuthorizationPolicies.FileRead)]
        public async Task<IActionResult> DownloadFile(Guid fileItemId)
        {
            var download = await _downloadFileUseCase.ExecuteAsync(fileItemId);

            if (download == null)
            {
                return NotFound();
            }

            var (fileItem, content) = download.Value;

            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                fileItemId,
                new OwnershipRequirement(ResourceType.File, AccessLevel.Read));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }
            await _fileActivityRepository.TrackAsync(
                User.GetUserId(),
                fileItemId,
                FileActivityType.Downloaded
            );

            return File(content, fileItem.FileType, fileItem.FileName);
        }

        [HttpPut("{fileItemId:guid}/rename")]
        [Authorize(Policy = AuthorizationPolicies.FileUpload)]
        public async Task<ActionResult<RenameFileResponseDto>> RenameFile(
            Guid fileItemId,
            RenameFileRequestDto dto)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                fileItemId,
                new OwnershipRequirement(ResourceType.File, AccessLevel.Write));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var fileItem = await _renameFileUseCase.ExecuteAsync(fileItemId, dto.NewName);

            if (fileItem == null)
            {
                return NotFound();
            }

            await _fileActivityRepository.TrackAsync(
                User.GetUserId(),
                fileItemId,
                FileActivityType.Renamed
            );
            await CreateFileNotificationAsync(
                "File renamed",
                $"Your file was renamed to {fileItem.FileName}.",
                fileItem.UserId,
                fileItem.FileItemId);

            return Ok(new RenameFileResponseDto { File = MapFile(fileItem) });
        }

        [HttpPut("{fileItemId:guid}/move")]
        [Authorize(Policy = AuthorizationPolicies.FileUpload)]
        public async Task<ActionResult<MoveFileResponseDto>> MoveFile(
            Guid fileItemId,
            MoveFileRequestDto dto)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                fileItemId,
                new OwnershipRequirement(ResourceType.File, AccessLevel.Write));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            await _fileActivityRepository.TrackAsync(
                User.GetUserId(),
                fileItemId,
                FileActivityType.Moved
            );

            try
            {
                var fileItem = await _moveFileUseCase.ExecuteAsync(fileItemId, dto.TargetFolderId);

                if (fileItem == null)
                {
                    return NotFound();
                }

                await CreateFileNotificationAsync(
                    "File moved",
                    $"{fileItem.FileName} was moved to another folder.",
                    fileItem.UserId,
                    fileItem.FileItemId);

                return Ok(new MoveFileResponseDto { File = MapFile(fileItem) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{fileItemId:guid}")]
        [Authorize(Policy = AuthorizationPolicies.FileDelete)]
        public async Task<ActionResult<DeleteFileResponseDto>> DeleteFile(Guid fileItemId)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                fileItemId,
                new OwnershipRequirement(ResourceType.File, AccessLevel.Delete));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var deleted = await _deleteFileUseCase.ExecuteAsync(fileItemId);

            if (!deleted)
            {
                return NotFound();
            }
            await _fileActivityRepository.TrackAsync(
                User.GetUserId(),
                fileItemId,
                FileActivityType.Deleted
            );
            await CreateFileNotificationAsync(
                "File deleted",
                "A file was moved to trash.",
                User.GetUserId(),
                fileItemId);

            return Ok(new DeleteFileResponseDto { Success = true });
        }

        [HttpPost("{fileItemId:guid}/share")]
        [Authorize(Policy = AuthorizationPolicies.FileShare)]
        public async Task<ActionResult<ShareFileResponseDto>> ShareFile(
            Guid fileItemId,
            ShareFileRequestDto dto)
        {
            var authorization = await _authorizationService.AuthorizeAsync(
                User,
                fileItemId,
                new OwnershipRequirement(ResourceType.File, AccessLevel.Admin));

            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var sharedLink = await _shareFileUseCase.ExecuteAsync(
                fileItemId,
                dto.Permission,
                dto.ExpiresAt);

            if (sharedLink == null)
            {
                return NotFound();
            }

            await _createNotificationUseCase.ExecuteAsync(
                "File shared",
                "A share link was created for one of your files.",
                NotificationType.FileShared,
                NotificationChannel.InApp,
                User.GetUserId(),
                User.GetUserId(),
                fileItemId,
                null);

            return Ok(MapShareFile(sharedLink));
        }

        private async Task CreateFileNotificationAsync(
            string title,
            string message,
            Guid userId,
            Guid fileItemId)
        {
            await _createNotificationUseCase.ExecuteAsync(
                title,
                message,
                NotificationType.FileUpdated,
                NotificationChannel.InApp,
                userId,
                User.GetUserId(),
                fileItemId,
                null);
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

        private static ShareFileResponseDto MapShareFile(SharedLink sharedLink)
        {
            return new ShareFileResponseDto
            {
                ShareLinkId = sharedLink.SharedLinkId,
                ShareUrl = $"/api/shared-links/token/{sharedLink.TokenUrl}",
                ExpiresAt = sharedLink.ExpirationDate
            };
        }
    }
}
