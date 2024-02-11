using Mango.Services.AuthAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public UserDto ToUserDto()
        {
            return new()
            {
                Email = Email,
                ID = Id,
                Name = Name,
                PhoneNumber = PhoneNumber
            };
        }
    }
}
