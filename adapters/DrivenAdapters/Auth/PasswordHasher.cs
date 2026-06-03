using Microsoft.AspNetCore.Identity;
using ports.DrivenPorts.Auth;

namespace adapters.DrivenAdapters.Auth
{
    public class PasswordHasherAdapter : IPasswordHasher
        {
            private readonly PasswordHasher<object> _hasher = new();

            public string HashPassword(string password)
            {
                return _hasher.HashPassword(null, password);
            }

            public bool VerifyPassword(string password, string hashedPassword)
            {
                var result = _hasher.VerifyHashedPassword(null, hashedPassword, password);
                return result == PasswordVerificationResult.Success;
            }
        }
}