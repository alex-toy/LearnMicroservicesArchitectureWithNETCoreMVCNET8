using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.Authentication.AuthHelpers.Handlers;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service.Authentication.RegistrationHelpers
{
    public class CheckRegisteredUserExists : GenericHandler<RegistrationRequestDto, RegistrationResponseDto>
    {
        public UserManager<ApplicationUser> _userManager { private get; set; }

        public override async Task<RegistrationResponseDto> HandleAsync(RegistrationRequestDto registrationRequestDto, ApplicationUser user)
        {
            try
            {
                IdentityResult result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (!result.Succeeded) return new RegistrationResponseDto()
                {
                    User = null,
                    Message = result.Errors.FirstOrDefault().Description
                };

                return await Proceed(registrationRequestDto, user);
            }
            catch (Exception ex)
            {
                return new RegistrationResponseDto()
                {
                    User = null,
                    Message = ex.Message
                };
            }
        }
    }
}
