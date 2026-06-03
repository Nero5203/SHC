using Domain.Entities.Users;
namespace ports.DrivenPorts.Auth
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}