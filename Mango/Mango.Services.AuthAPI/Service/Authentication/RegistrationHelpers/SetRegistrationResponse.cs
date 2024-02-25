using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.Authentication.AuthHelpers.Handlers;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service.Authentication.RegistrationHelpers
{
    public class SetRegistrationResponse : GenericHandler<RegistrationRequestDto, RegistrationResponseDto>
    {
        public AuthDbContext _db { private get; set; }

        public override async Task<RegistrationResponseDto> HandleAsync(RegistrationRequestDto registrationRequestDto, ApplicationUser user)
        {
            ApplicationUser userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

            var response = new RegistrationResponseDto()
            {
                User = userToReturn.ToUserDto(),
                Message = string.Empty,
            };

            return response ?? await Proceed(registrationRequestDto, user);
        }
    }
}
