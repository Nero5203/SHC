using application.Dto.Permissions;
using application.Ports.Driving.Permissions;
using Microsoft.AspNetCore.Mvc;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace api.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    public class PermissionsController : ControllerBase
    {
        private readonly IGrantPermissionUseCase _grantPermissionUseCase;
        private readonly IUpdatePermissionUseCase _updatePermissionUseCase;
        private readonly IRevokePermissionUseCase _revokePermissionUseCase;
        private readonly ICheckPermissionUseCase _checkPermissionUseCase;
        private readonly IGetPermissionsBySubjectUseCase _getPermissionsBySubjectUseCase;
        private readonly IGetPermissionsByResourceUseCase _getPermissionsByResourceUseCase;

        public PermissionsController(
            IGrantPermissionUseCase grantPermissionUseCase,
            IUpdatePermissionUseCase updatePermissionUseCase,
            IRevokePermissionUseCase revokePermissionUseCase,
            ICheckPermissionUseCase checkPermissionUseCase,
            IGetPermissionsBySubjectUseCase getPermissionsBySubjectUseCase,
            IGetPermissionsByResourceUseCase getPermissionsByResourceUseCase)
        {
            _grantPermissionUseCase = grantPermissionUseCase;
            _updatePermissionUseCase = updatePermissionUseCase;
            _revokePermissionUseCase = revokePermissionUseCase;
            _checkPermissionUseCase = checkPermissionUseCase;
            _getPermissionsBySubjectUseCase = getPermissionsBySubjectUseCase;
            _getPermissionsByResourceUseCase = getPermissionsByResourceUseCase;
        }

        [HttpPost]
        public async Task<ActionResult<PermissionResponseDto>> GrantPermission(GrantPermissionDto dto)
        {
            try
            {
                var permission = await _grantPermissionUseCase.ExecuteAsync(
                    dto.SubjectId,
                    dto.SubjectType,
                    dto.ResourceId,
                    dto.ResourceType,
                    dto.AccessLevel,
                    dto.GrantedBySubjectId,
                    dto.GrantedBySubjectType);

                return CreatedAtAction(
                    nameof(GetPermissionsBySubject),
                    new { subjectType = permission.SubjectType, subjectId = permission.SubjectId },
                    MapPermission(permission));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("subject/{subjectType}/{subjectId:guid}")]
        public async Task<ActionResult<IReadOnlyList<PermissionResponseDto>>> GetPermissionsBySubject(
            SubjectType subjectType,
            Guid subjectId)
        {
            var permissions = await _getPermissionsBySubjectUseCase.ExecuteAsync(subjectType, subjectId);

            var response = permissions
                .Select(MapPermission)
                .ToList();

            return Ok(response);
        }

        [HttpGet("resource/{resourceType}/{resourceId:guid}")]
        public async Task<ActionResult<IReadOnlyList<PermissionResponseDto>>> GetPermissionsByResource(
            ResourceType resourceType,
            Guid resourceId)
        {
            var permissions = await _getPermissionsByResourceUseCase.ExecuteAsync(resourceType, resourceId);

            var response = permissions
                .Select(MapPermission)
                .ToList();

            return Ok(response);
        }

        [HttpPut("{permissionId:guid}")]
        public async Task<ActionResult<PermissionResponseDto>> UpdatePermission(
            Guid permissionId,
            UpdatePermissionDto dto)
        {
            try
            {
                var permission = await _updatePermissionUseCase.ExecuteAsync(
                    permissionId,
                    dto.AccessLevel,
                    dto.GrantedBySubjectId,
                    dto.GrantedBySubjectType);

                if (permission == null)
                {
                    return NotFound();
                }

                return Ok(MapPermission(permission));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{permissionId:guid}")]
        public async Task<IActionResult> RevokePermission(
            Guid permissionId,
            [FromQuery] Guid? revokedBySubjectId,
            [FromQuery] SubjectType? revokedBySubjectType)
        {
            var revoked = await _revokePermissionUseCase.ExecuteAsync(
                permissionId,
                revokedBySubjectId,
                revokedBySubjectType);

            if (!revoked)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("check")]
        public async Task<ActionResult<CheckPermissionResponseDto>> CheckPermission(CheckPermissionDto dto)
        {
            var result = await _checkPermissionUseCase.ExecuteAsync(
                dto.SubjectId,
                dto.SubjectType,
                dto.ResourceId,
                dto.ResourceType,
                dto.RequiredAccessLevel);

            return Ok(new CheckPermissionResponseDto
            {
                HasAccess = result.HasAccess,
                RequiredAccessLevel = dto.RequiredAccessLevel,
                GrantedAccessLevel = result.GrantedAccessLevel
            });
        }

        private static PermissionResponseDto MapPermission(Permission permission)
        {
            return new PermissionResponseDto
            {
                PermissionId = permission.PermissionId,
                SubjectId = permission.SubjectId,
                SubjectType = permission.SubjectType,
                ResourceId = permission.ResourceId,
                ResourceType = permission.ResourceType,
                AccessLevel = permission.AccessLevel,
                GrantedAtUtc = permission.GrantedAtUtc,
                GrantedBySubjectId = permission.GrantedBySubjectId,
                GrantedBySubjectType = permission.GrantedBySubjectType
            };
        }
    }
}
