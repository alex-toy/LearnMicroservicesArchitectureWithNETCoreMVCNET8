using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.Authentication.AuthHelpers;
using Mango.Services.AuthAPI.Service.Authentication.RegistrationHelpers;
using Mango.Services.AuthAPI.Service.JwtToken;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private CheckUserExists _loginProcessors;
        private CheckRegisteredUserExists _registrationProcessors;

        public AuthService(AuthDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;

            SetLoginProcessors();
            SetRegistrationProcessors();
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            ApplicationUser? user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user is null) return false;

            bool roleExists = _roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();
            if (!roleExists)
            {
                _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }

            await _userManager.AddToRoleAsync(user, roleName);

            return true;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            ApplicationUser? applicationUser = GetApplicationUser(loginRequestDto);

            return await _loginProcessors.HandleAsync(loginRequestDto, applicationUser);
        }

        public async Task<RegistrationResponseDto> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = registrationRequestDto.ToApplicationUser();
            return await _registrationProcessors.HandleAsync(registrationRequestDto, user);
        }

        private void SetLoginProcessors()
        {
            _loginProcessors = new CheckUserExists();
            _loginProcessors
                .SetNext(new CheckPassword() { _userManager = _userManager })
                .SetNext(new GenerateToken() { _userManager = _userManager, _jwtTokenGenerator = _jwtTokenGenerator });
        }

        private void SetRegistrationProcessors()
        {
            _registrationProcessors = new CheckRegisteredUserExists() { _userManager = _userManager };
            _registrationProcessors.SetNext(new SetRegistrationResponse() { _db = _db });
        }

        private ApplicationUser? GetApplicationUser(LoginRequestDto loginRequestDto)
        {
            return _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
        }
    }
}
