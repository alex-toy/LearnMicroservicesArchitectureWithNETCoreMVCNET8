using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Service.Authentication
{
    public interface IAuthService
    {
        Task<RegistrationResponseDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}
