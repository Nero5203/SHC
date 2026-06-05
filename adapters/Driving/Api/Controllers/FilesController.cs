using api.Requests.FileStorage;
using application.Dto.FileStorage.File;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.FileStorage;
using Domain.Entities.LinkSharing;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
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

        public FilesController(
            IUploadFileUseCase uploadFileUseCase,
            IGetFileByIdUseCase getFileByIdUseCase,
            ISearchFilesUseCase searchFilesUseCase,
            IDownloadFileUseCase downloadFileUseCase,
            IRenameFileUseCase renameFileUseCase,
            IMoveFileUseCase moveFileUseCase,
            IDeleteFileUseCase deleteFileUseCase,
            IShareFileUseCase shareFileUseCase)
        {
            _uploadFileUseCase = uploadFileUseCase;
            _getFileByIdUseCase = getFileByIdUseCase;
            _searchFilesUseCase = searchFilesUseCase;
            _downloadFileUseCase = downloadFileUseCase;
            _renameFileUseCase = renameFileUseCase;
            _moveFileUseCase = moveFileUseCase;
            _deleteFileUseCase = deleteFileUseCase;
            _shareFileUseCase = shareFileUseCase;
        }

        [HttpPost("upload")]
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
        public async Task<ActionResult<FileDto>> GetFileById(Guid fileItemId)
        {
            var fileItem = await _getFileByIdUseCase.ExecuteAsync(fileItemId);

            if (fileItem == null)
            {
                return NotFound();
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
        public async Task<IActionResult> DownloadFile(Guid fileItemId)
        {
            var download = await _downloadFileUseCase.ExecuteAsync(fileItemId);

            if (download == null)
            {
                return NotFound();
            }

            var (fileItem, content) = download.Value;

            return File(content, fileItem.FileType, fileItem.FileName);
        }

        [HttpPut("{fileItemId:guid}/rename")]
        public async Task<ActionResult<RenameFileResponseDto>> RenameFile(
            Guid fileItemId,
            RenameFileRequestDto dto)
        {
            var fileItem = await _renameFileUseCase.ExecuteAsync(fileItemId, dto.NewName);

            if (fileItem == null)
            {
                return NotFound();
            }

            return Ok(new RenameFileResponseDto { File = MapFile(fileItem) });
        }

        [HttpPut("{fileItemId:guid}/move")]
        public async Task<ActionResult<MoveFileResponseDto>> MoveFile(
            Guid fileItemId,
            MoveFileRequestDto dto)
        {
            try
            {
                var fileItem = await _moveFileUseCase.ExecuteAsync(fileItemId, dto.TargetFolderId);

                if (fileItem == null)
                {
                    return NotFound();
                }

                return Ok(new MoveFileResponseDto { File = MapFile(fileItem) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{fileItemId:guid}")]
        public async Task<ActionResult<DeleteFileResponseDto>> DeleteFile(Guid fileItemId)
        {
            var deleted = await _deleteFileUseCase.ExecuteAsync(fileItemId);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok(new DeleteFileResponseDto { Success = true });
        }

        [HttpPost("{fileItemId:guid}/share")]
        public async Task<ActionResult<ShareFileResponseDto>> ShareFile(
            Guid fileItemId,
            ShareFileRequestDto dto)
        {
            var sharedLink = await _shareFileUseCase.ExecuteAsync(
                fileItemId,
                dto.Permission,
                dto.ExpiresAt);

            if (sharedLink == null)
            {
                return NotFound();
            }

            return Ok(MapShareFile(sharedLink));
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
