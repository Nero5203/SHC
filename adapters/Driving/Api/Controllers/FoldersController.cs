using application.Dto.FileStorage.File;
using application.Dto.FileStorage.Folder;
using application.Ports.Driving.FileStorage.Folder;
using Domain.Entities.FileStorage;
using Domain.Entities.LinkSharing;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
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

        public FoldersController(
            ICreateFolderUseCase createFolderUseCase,
            IGetFolderByIdUseCase getFolderByIdUseCase,
            IListFolderContentUseCase listFolderContentUseCase,
            IRenameFolderUseCase renameFolderUseCase,
            IMoveFolderUseCase moveFolderUseCase,
            IDeleteFolderUseCase deleteFolderUseCase,
            IArchiveFolderUseCase archiveFolderUseCase,
            IShareFolderUseCase shareFolderUseCase)
        {
            _createFolderUseCase = createFolderUseCase;
            _getFolderByIdUseCase = getFolderByIdUseCase;
            _listFolderContentUseCase = listFolderContentUseCase;
            _renameFolderUseCase = renameFolderUseCase;
            _moveFolderUseCase = moveFolderUseCase;
            _deleteFolderUseCase = deleteFolderUseCase;
            _archiveFolderUseCase = archiveFolderUseCase;
            _shareFolderUseCase = shareFolderUseCase;
        }

        [HttpPost]
        public async Task<ActionResult<CreateFolderResponseDto>> CreateFolder(CreateFolderRequestDto dto)
        {
            try
            {
                var folder = await _createFolderUseCase.ExecuteAsync(
                    dto.UserId,
                    dto.Name,
                    dto.ParentFolderId);

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
        public async Task<ActionResult<FolderDto>> GetFolderById(Guid folderId)
        {
            var folder = await _getFolderByIdUseCase.ExecuteAsync(folderId);

            if (folder == null)
            {
                return NotFound();
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
        public async Task<ActionResult<RenameFolderResponseDto>> RenameFolder(
            Guid folderId,
            RenameFolderRequestDto dto)
        {
            var folder = await _renameFolderUseCase.ExecuteAsync(folderId, dto.NewName);

            if (folder == null)
            {
                return NotFound();
            }

            return Ok(new RenameFolderResponseDto { Folder = MapFolder(folder) });
        }

        [HttpPut("{folderId:guid}/move")]
        public async Task<ActionResult<MoveFolderResponseDto>> MoveFolder(
            Guid folderId,
            MoveFolderRequestDto dto)
        {
            try
            {
                var folder = await _moveFolderUseCase.ExecuteAsync(folderId, dto.TargetParentFolderId);

                if (folder == null)
                {
                    return NotFound();
                }

                return Ok(new MoveFolderResponseDto { Folder = MapFolder(folder) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{folderId:guid}/archive")]
        public async Task<ActionResult<FolderDto>> ArchiveFolder(Guid folderId)
        {
            var folder = await _archiveFolderUseCase.ExecuteAsync(folderId);

            if (folder == null)
            {
                return NotFound();
            }

            return Ok(MapFolder(folder));
        }

        [HttpDelete("{folderId:guid}")]
        public async Task<ActionResult<DeleteFolderResponseDto>> DeleteFolder(Guid folderId)
        {
            var deleted = await _deleteFolderUseCase.ExecuteAsync(folderId);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok(new DeleteFolderResponseDto { Success = true });
        }

        [HttpPost("{folderId:guid}/share")]
        public async Task<ActionResult<ShareFolderResponseDto>> ShareFolder(
            Guid folderId,
            ShareFolderRequestDto dto)
        {
            var sharedLink = await _shareFolderUseCase.ExecuteAsync(
                folderId,
                dto.Permission,
                dto.ExpiresAt);

            if (sharedLink == null)
            {
                return NotFound();
            }

            return Ok(MapShareFolder(sharedLink));
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
