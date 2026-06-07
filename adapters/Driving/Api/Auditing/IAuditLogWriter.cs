using System.Security.Claims;
using SHC.Domain.Entities.Permissions.Enums;

namespace api.Auditing
{
    public interface IAuditLogWriter
    {
        Task WriteAsync(
            ClaimsPrincipal user,
            string action,
            ResourceType resourceType,
            string resourceId,
            bool isSuccess = true,
            object? payload = null);
    }
}
