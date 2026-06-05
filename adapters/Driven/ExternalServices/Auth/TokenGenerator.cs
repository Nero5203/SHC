using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using application.Common.Authorization;
using application.Ports.Driven.Roles;
using Domain.Entities.Users;
using Microsoft.IdentityModel.Tokens;
using application.Ports.Driven.Auth;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace adapters.Driven.ExternalServices.Auth
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;

        public JwtTokenGenerator(IConfiguration configuration, IRoleRepository roleRepository)
        {
            _configuration = configuration;
            _roleRepository = roleRepository;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var userRoles = await _roleRepository.GetUserRolesWithPermissionsAsync(user.UserId);
            var roles = userRoles
                .Select(userRole => userRole.Role.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var permissions = userRoles
                .SelectMany(userRole => userRole.Role.RolePermissions.Select(rolePermission => rolePermission.Permission.Name))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return GenerateToken(user, roles, permissions);
        }

        public string GenerateToken(User user)
        {
            return GenerateToken(user, Array.Empty<string>(), Array.Empty<string>());
        }

        private string GenerateToken(User user, IReadOnlyCollection<string> roles, IReadOnlyCollection<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(permissions.Select(permission => new Claim(ShcClaimTypes.Permission, permission)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            
            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );
        
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}   
