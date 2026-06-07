using System.Security.Claims;

namespace api.Authorization
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(value, out var userId))
            {
                return userId;
            }

            throw new InvalidOperationException("Authenticated user id claim is missing.");
        }
    }
}
