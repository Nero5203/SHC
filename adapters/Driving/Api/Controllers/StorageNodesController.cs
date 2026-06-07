using api.Auditing;
using application.Common.Authorization;
using application.Dto.StorageNodes;
using Domain.Entities.StorageNodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using application.Ports.Driving.StorageNodes;
using SHC.Domain.Entities.Permissions.Enums;

namespace api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/storage-nodes")]
    public class StorageNodesController : ControllerBase
    {
        private readonly ICreateStorageNodeUseCase _createStorageNodeUseCase;
        private readonly IGetStorageNodeByIdUseCase _getStorageNodeByIdUseCase;
        private readonly IGetStorageNodesUseCase _getStorageNodesUseCase;
        private readonly IGetBestAvailableStorageNodeUseCase _getBestAvailableStorageNodeUseCase;
        private readonly IUpdateStorageNodeUseCase _updateStorageNodeUseCase;
        private readonly IUpdateStorageNodeHeartbeatUseCase _updateStorageNodeHeartbeatUseCase;
        private readonly IUpdateStorageNodeStatusUseCase _updateStorageNodeStatusUseCase;
        private readonly IAuditLogWriter _auditLogWriter;

        public StorageNodesController(
            ICreateStorageNodeUseCase createStorageNodeUseCase,
            IGetStorageNodeByIdUseCase getStorageNodeByIdUseCase,
            IGetStorageNodesUseCase getStorageNodesUseCase,
            IGetBestAvailableStorageNodeUseCase getBestAvailableStorageNodeUseCase,
            IUpdateStorageNodeUseCase updateStorageNodeUseCase,
            IUpdateStorageNodeHeartbeatUseCase updateStorageNodeHeartbeatUseCase,
            IUpdateStorageNodeStatusUseCase updateStorageNodeStatusUseCase,
            IAuditLogWriter auditLogWriter)
        {
            _createStorageNodeUseCase = createStorageNodeUseCase;
            _getStorageNodeByIdUseCase = getStorageNodeByIdUseCase;
            _getStorageNodesUseCase = getStorageNodesUseCase;
            _getBestAvailableStorageNodeUseCase = getBestAvailableStorageNodeUseCase;
            _updateStorageNodeUseCase = updateStorageNodeUseCase;
            _updateStorageNodeHeartbeatUseCase = updateStorageNodeHeartbeatUseCase;
            _updateStorageNodeStatusUseCase = updateStorageNodeStatusUseCase;
            _auditLogWriter = auditLogWriter;
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.NodeManage)]
        public async Task<ActionResult<StorageNodeResponseDto>> CreateStorageNode(CreateStorageNodeDto dto)
        {
            var storageNode = await _createStorageNodeUseCase.ExecuteAsync(
                dto.Name,
                dto.Hostname,
                dto.IpAddress,
                dto.Port,
                dto.BasePath,
                dto.TotalCapacityBytes);

            await WriteStorageNodeAuditAsync("StorageNode.Created", storageNode);

            return CreatedAtAction(
                nameof(GetStorageNodeById),
                new { storageNodeId = storageNode.StorageNodeId },
                MapStorageNode(storageNode));
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.NodeManage)]
        public async Task<ActionResult<IReadOnlyList<StorageNodeResponseDto>>> GetStorageNodes()
        {
            var storageNodes = await _getStorageNodesUseCase.ExecuteAsync();

            var response = storageNodes
                .Select(MapStorageNode)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{storageNodeId:guid}")]
        [Authorize(Policy = AuthorizationPolicies.NodeManage)]
        public async Task<ActionResult<StorageNodeResponseDto>> GetStorageNodeById(Guid storageNodeId)
        {
            var storageNode = await _getStorageNodeByIdUseCase.ExecuteAsync(storageNodeId);

            if (storageNode == null)
            {
                return NotFound();
            }

            await WriteStorageNodeAuditAsync("StorageNode.Updated", storageNode);

            return Ok(MapStorageNode(storageNode));
        }

        [HttpGet("available")]
        [Authorize(Policy = AuthorizationPolicies.FileUpload)]
        public async Task<ActionResult<StorageNodeResponseDto>> GetBestAvailableStorageNode(
            [FromQuery] long requiredBytes)
        {
            var storageNode = await _getBestAvailableStorageNodeUseCase.ExecuteAsync(requiredBytes);

            if (storageNode == null)
            {
                return NotFound();
            }

            await WriteStorageNodeAuditAsync("StorageNode.HeartbeatUpdated", storageNode);

            return Ok(MapStorageNode(storageNode));
        }

        [HttpPut("{storageNodeId:guid}")]
        [Authorize(Policy = AuthorizationPolicies.NodeManage)]
        public async Task<ActionResult<StorageNodeResponseDto>> UpdateStorageNode(
            Guid storageNodeId,
            UpdateStorageNodeDto dto)
        {
            var storageNode = await _updateStorageNodeUseCase.ExecuteAsync(
                storageNodeId,
                dto.Name,
                dto.Hostname,
                dto.IpAddress,
                dto.Port,
                dto.BasePath,
                dto.TotalCapacityBytes);

            if (storageNode == null)
            {
                return NotFound();
            }

            return Ok(MapStorageNode(storageNode));
        }

        [HttpPut("{storageNodeId:guid}/heartbeat")]
        [Authorize(Policy = AuthorizationPolicies.NodeManage)]
        public async Task<ActionResult<StorageNodeResponseDto>> UpdateStorageNodeHeartbeat(
            Guid storageNodeId,
            StorageNodeHeartbeatDto dto)
        {
            var storageNode = await _updateStorageNodeHeartbeatUseCase.ExecuteAsync(
                storageNodeId,
                dto.TotalCapacityBytes,
                dto.UsedCapacityBytes,
                dto.Status);

            if (storageNode == null)
            {
                return NotFound();
            }

            return Ok(MapStorageNode(storageNode));
        }

        [HttpPut("{storageNodeId:guid}/status")]
        [Authorize(Policy = AuthorizationPolicies.NodeManage)]
        public async Task<ActionResult<StorageNodeResponseDto>> UpdateStorageNodeStatus(
            Guid storageNodeId,
            UpdateStorageNodeStatusDto dto)
        {
            var storageNode = await _updateStorageNodeStatusUseCase.ExecuteAsync(
                storageNodeId,
                dto.Status);

            if (storageNode == null)
            {
                return NotFound();
            }

            await WriteStorageNodeAuditAsync("StorageNode.StatusUpdated", storageNode);

            return Ok(MapStorageNode(storageNode));
        }

        private async Task WriteStorageNodeAuditAsync(string action, StorageNode storageNode)
        {
            await _auditLogWriter.WriteAsync(
                User,
                action,
                ResourceType.StorageNode,
                storageNode.StorageNodeId.ToString(),
                true,
                new
                {
                    storageNode.Name,
                    storageNode.Hostname,
                    storageNode.IpAddress,
                    storageNode.Port,
                    storageNode.Status,
                    storageNode.UsedCapacityBytes,
                    storageNode.TotalCapacityBytes
                });
        }

        private static StorageNodeResponseDto MapStorageNode(StorageNode storageNode)
        {
            return new StorageNodeResponseDto
            {
                StorageNodeId = storageNode.StorageNodeId,
                Name = storageNode.Name,
                Hostname = storageNode.Hostname,
                IpAddress = storageNode.IpAddress,
                Port = storageNode.Port,
                BasePath = storageNode.BasePath,
                TotalCapacityBytes = storageNode.TotalCapacityBytes,
                UsedCapacityBytes = storageNode.UsedCapacityBytes,
                Status = storageNode.Status,
                CreatedAt = storageNode.CreatedAt,
                UpdatedAt = storageNode.UpdatedAt,
                LastHeartbeatAt = storageNode.LastHeartbeatAt,
                FileCount = storageNode.FileItems.Count
            };
        }
    }
}
