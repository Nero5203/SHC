using application.Dto.Permissions;
using application.Ports.Driving.Permissions;
using Microsoft.AspNetCore.Mvc;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace api.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IListAuditLogsUseCase _listAuditLogsUseCase;

        public AuditLogsController(IListAuditLogsUseCase listAuditLogsUseCase)
        {
            _listAuditLogsUseCase = listAuditLogsUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AuditLogResponseDto>>> ListAuditLogs()
        {
            var auditLogs = await _listAuditLogsUseCase.ExecuteAsync(null, null, null, null);

            return Ok(MapAuditLogs(auditLogs));
        }

        [HttpGet("subject/{subjectType}/{subjectId:guid}")]
        public async Task<ActionResult<IReadOnlyList<AuditLogResponseDto>>> GetAuditLogsBySubject(
            SubjectType subjectType,
            Guid subjectId)
        {
            var auditLogs = await _listAuditLogsUseCase.ExecuteAsync(subjectType, subjectId, null, null);

            return Ok(MapAuditLogs(auditLogs));
        }

        [HttpGet("resource/{resourceType}/{resourceId:guid}")]
        public async Task<ActionResult<IReadOnlyList<AuditLogResponseDto>>> GetAuditLogsByResource(
            ResourceType resourceType,
            Guid resourceId)
        {
            var auditLogs = await _listAuditLogsUseCase.ExecuteAsync(null, null, resourceType, resourceId);

            return Ok(MapAuditLogs(auditLogs));
        }

        private static IReadOnlyList<AuditLogResponseDto> MapAuditLogs(IReadOnlyList<AuditLog> auditLogs)
        {
            return auditLogs
                .Select(MapAuditLog)
                .ToList();
        }

        private static AuditLogResponseDto MapAuditLog(AuditLog auditLog)
        {
            return new AuditLogResponseDto
            {
                AuditLogId = auditLog.AuditLogId,
                TimestampUtc = auditLog.TimestampUtc,
                SubjectId = auditLog.SubjectId,
                SubjectType = auditLog.SubjectType,
                Action = auditLog.Action,
                ResourceType = auditLog.ResourceType,
                ResourceId = auditLog.ResourceId,
                IsSuccess = auditLog.IsSuccess,
                PayloadJson = auditLog.PayloadJson
            };
        }
    }
}
