using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.Authentication.AuthHelpers.Handlers;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service.Authentication.AuthHelpers
{
    public class CheckPassword : GenericHandler<LoginRequestDto, LoginResponseDto>
    {
        public UserManager<ApplicationUser> _userManager { private get; set; }

        public override async Task<LoginResponseDto> HandleAsync(LoginRequestDto loginRequestDto, ApplicationUser user)
        {
            bool passwordIsValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!passwordIsValid) return new LoginResponseDto() { User = null, Token = "" };

            return await Proceed(loginRequestDto, user);
        }
    }
}
