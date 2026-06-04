using Domain.Entities.Users;
namespace application.Ports.Driven.Auth
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
    }
}