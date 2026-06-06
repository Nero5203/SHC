using application.Common.Authorization;
using application.Ports.Driven;
using application.Ports.Driven.Auth;
using application.Ports.Driven.Roles;
using application.Ports.Driving.Auth;
using application.UseCases.Auth;
using Domain.Entities.Auth;
using Domain.Entities.Roles;
using Domain.Entities.Users;

namespace Application.UseCases.Auth
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserCredentialRepository _credentialRepo;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserUseCase(
            IUserRepository userRepo,
            IUserCredentialRepository credentialRepo,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _credentialRepo = credentialRepo;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task RegisterUserAsync(RegisterUserRequest registerUserRequest)
        {
            var existingUser = await _userRepo.GetByEmailAsync(registerUserRequest.Email);

            if (existingUser != null)
            {
                throw new Exception("User with this email already exists.");
            }

            var userRole = await _roleRepository.GetByNameAsync(AuthorizationRoles.User);

            if (userRole == null)
            {
                throw new InvalidOperationException("Default user role was not found.");
            }

            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var user = new User
            {
                UserId = userId,
                Email = registerUserRequest.Email,
                Username = registerUserRequest.Username,
                FirstName = registerUserRequest.FirstName,
                LastName = registerUserRequest.LastName,
                PhoneNumber = registerUserRequest.PhoneNumber
            };   

            var credential = new UserCredential
            {
                UserCredentialId = Guid.NewGuid(),
                UserId = userId,
                PasswordHash = _passwordHasher.HashPassword(registerUserRequest.Password),
                CreatedAt = now,
                UpdatedAt = now
            };

            var roleAssignment = new UserRole
            {
                UserId = userId,
                RoleId = userRole.RoleId,
                AssignedAt = now
            };

            await _userRepo.CreateAsync(user);
            await _credentialRepo.CreateAsync(credential);
            await _roleRepository.AssignRoleToUserAsync(roleAssignment);
        }
    }
}
