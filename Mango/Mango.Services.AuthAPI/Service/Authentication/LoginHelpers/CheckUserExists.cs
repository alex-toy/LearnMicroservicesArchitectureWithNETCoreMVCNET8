using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.Authentication.AuthHelpers.Handlers;

namespace Mango.Services.AuthAPI.Service.Authentication.AuthHelpers
{
    public class CheckUserExists : GenericHandler<LoginRequestDto, LoginResponseDto>
    {
        public override async Task<LoginResponseDto> HandleAsync(LoginRequestDto loginRequestDto, ApplicationUser user)
        {
            if (user is null) return new LoginResponseDto() { User = null, Token = "" };

            return await Proceed(loginRequestDto, user);
        }
    }
}
