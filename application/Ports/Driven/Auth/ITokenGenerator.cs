using Domain.Entities.Users;
namespace application.Ports.Driven.Auth
{
    public interface ITokenGenerator
    {
        Task<string> GenerateTokenAsync(User user);
        string GenerateToken(User user);
        string GenerateRefreshToken();
    }
}
