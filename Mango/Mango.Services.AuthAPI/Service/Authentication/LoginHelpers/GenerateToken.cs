using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.Authentication.AuthHelpers.Handlers;
using Mango.Services.AuthAPI.Service.JwtToken;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service.Authentication.AuthHelpers
{
    public class GenerateToken : GenericHandler<LoginRequestDto, LoginResponseDto>
    {
        public UserManager<ApplicationUser> _userManager { private get; set; }
        public IJwtTokenGenerator _jwtTokenGenerator { private get; set; }

        public override async Task<LoginResponseDto> HandleAsync(LoginRequestDto loginRequestDto, ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            string token = _jwtTokenGenerator.GenerateToken(user, roles);

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = user.ToUserDto(),
                Token = token
            };

            return loginResponseDto ?? await Proceed(loginRequestDto, user);
        }
    }
}
