using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Service.JwtToken
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
