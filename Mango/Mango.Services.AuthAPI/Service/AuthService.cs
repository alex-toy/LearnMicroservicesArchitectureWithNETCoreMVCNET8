using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AuthDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            ApplicationUser? user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user is not null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }

            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            ApplicationUser? applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            bool passwordIsValid = await _userManager.CheckPasswordAsync(applicationUser, loginRequestDto.Password);

            if(applicationUser is null || !passwordIsValid )
            {
                return new LoginResponseDto() { User = null, Token="" };
            }

            var roles = await _userManager.GetRolesAsync(applicationUser);
            var token = _jwtTokenGenerator.GenerateToken(applicationUser, roles);

            UserDto userDTO = applicationUser.ToUserDto();

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDTO,
                Token = token
            };

            return loginResponseDto;
        }

        public async Task<RegistrationResponseDto> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = registrationRequestDto.ToApplicationUser();

            try
            {
                IdentityResult result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

                if (!result.Succeeded) return new RegistrationResponseDto()
                {
                    User = null,
                    Message = result.Errors.FirstOrDefault().Description
                };

                ApplicationUser userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                UserDto userDto = userToReturn.ToUserDto();

                return new RegistrationResponseDto()
                {
                    User = userDto,
                    Message = string.Empty,
                };
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
