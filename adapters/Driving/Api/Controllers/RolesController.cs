using application.Dto.Roles;
using application.Ports.Driving.Roles;
using Domain.Entities.Roles;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly ICreateRoleUseCase _createRoleUseCase;
        private readonly IGetRoleByIdUseCase _getRoleByIdUseCase;
        private readonly IListRolesUseCase _listRolesUseCase;
        private readonly IUpdateRoleUseCase _updateRoleUseCase;
        private readonly IDeleteRoleUseCase _deleteRoleUseCase;
        private readonly IAssignRoleToUserUseCase _assignRoleToUserUseCase;
        private readonly IRemoveRoleFromUserUseCase _removeRoleFromUserUseCase;
        private readonly IGetUserRolesUseCase _getUserRolesUseCase;
        private readonly IGetRoleUsersUseCase _getRoleUsersUseCase;

        public RolesController(
            ICreateRoleUseCase createRoleUseCase,
            IGetRoleByIdUseCase getRoleByIdUseCase,
            IListRolesUseCase listRolesUseCase,
            IUpdateRoleUseCase updateRoleUseCase,
            IDeleteRoleUseCase deleteRoleUseCase,
            IAssignRoleToUserUseCase assignRoleToUserUseCase,
            IRemoveRoleFromUserUseCase removeRoleFromUserUseCase,
            IGetUserRolesUseCase getUserRolesUseCase,
            IGetRoleUsersUseCase getRoleUsersUseCase)
        {
            _createRoleUseCase = createRoleUseCase;
            _getRoleByIdUseCase = getRoleByIdUseCase;
            _listRolesUseCase = listRolesUseCase;
            _updateRoleUseCase = updateRoleUseCase;
            _deleteRoleUseCase = deleteRoleUseCase;
            _assignRoleToUserUseCase = assignRoleToUserUseCase;
            _removeRoleFromUserUseCase = removeRoleFromUserUseCase;
            _getUserRolesUseCase = getUserRolesUseCase;
            _getRoleUsersUseCase = getRoleUsersUseCase;
        }

        [HttpPost]
        public async Task<ActionResult<RoleResponseDto>> CreateRole(CreateRoleDto dto)
        {
            try
            {
                var role = await _createRoleUseCase.ExecuteAsync(dto.Name, dto.Description);

                return CreatedAtAction(
                    nameof(GetRoleById),
                    new { roleId = role.RoleId },
                    MapRole(role));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<RoleResponseDto>>> ListRoles()
        {
            var roles = await _listRolesUseCase.ExecuteAsync();

            var response = roles
                .Select(MapRole)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{roleId:guid}")]
        public async Task<ActionResult<RoleResponseDto>> GetRoleById(Guid roleId)
        {
            var role = await _getRoleByIdUseCase.ExecuteAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(MapRole(role));
        }

        [HttpPut("{roleId:guid}")]
        public async Task<ActionResult<RoleResponseDto>> UpdateRole(Guid roleId, UpdateRoleDto dto)
        {
            try
            {
                var role = await _updateRoleUseCase.ExecuteAsync(roleId, dto.Name, dto.Description);

                if (role == null)
                {
                    return NotFound();
                }

                return Ok(MapRole(role));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{roleId:guid}")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            var deleted = await _deleteRoleUseCase.ExecuteAsync(roleId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{roleId:guid}/users/{userId:guid}")]
        public async Task<ActionResult<UserRoleResponseDto>> AssignRoleToUser(Guid roleId, Guid userId)
        {
            var userRole = await _assignRoleToUserUseCase.ExecuteAsync(roleId, userId);

            if (userRole == null)
            {
                return NotFound();
            }

            return Ok(MapUserRole(userRole));
        }

        [HttpDelete("{roleId:guid}/users/{userId:guid}")]
        public async Task<IActionResult> RemoveRoleFromUser(Guid roleId, Guid userId)
        {
            var removed = await _removeRoleFromUserUseCase.ExecuteAsync(roleId, userId);

            if (!removed)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<UserRoleResponseDto>>> GetUserRoles(Guid userId)
        {
            var userRoles = await _getUserRolesUseCase.ExecuteAsync(userId);

            var response = userRoles
                .Select(MapUserRole)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{roleId:guid}/users")]
        public async Task<ActionResult<IReadOnlyList<RoleUserResponseDto>>> GetRoleUsers(Guid roleId)
        {
            var userRoles = await _getRoleUsersUseCase.ExecuteAsync(roleId);

            var response = userRoles
                .Select(MapRoleUser)
                .ToList();

            return Ok(response);
        }

        private static RoleResponseDto MapRole(Role role)
        {
            return new RoleResponseDto
            {
                RoleId = role.RoleId,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UserCount = role.UserRoles.Count
            };
        }

        private static UserRoleResponseDto MapUserRole(UserRole userRole)
        {
            return new UserRoleResponseDto
            {
                UserId = userRole.UserId,
                RoleId = userRole.RoleId,
                RoleName = userRole.Role?.Name ?? string.Empty,
                RoleDescription = userRole.Role?.Description ?? string.Empty,
                AssignedAt = userRole.AssignedAt
            };
        }

        private static RoleUserResponseDto MapRoleUser(UserRole userRole)
        {
            return new RoleUserResponseDto
            {
                UserId = userRole.UserId,
                Username = userRole.User?.Username ?? string.Empty,
                FirstName = userRole.User?.FirstName ?? string.Empty,
                LastName = userRole.User?.LastName ?? string.Empty,
                Email = userRole.User?.Email ?? string.Empty,
                AssignedAt = userRole.AssignedAt
            };
        }
    }
}
