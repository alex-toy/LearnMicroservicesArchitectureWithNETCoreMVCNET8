using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IServiceBus _serviceBus;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService, IConfiguration configuration, IServiceBus serviceBus)
        {
            _authService = authService;
            _configuration = configuration;
            _response = new();
            _serviceBus = serviceBus;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            RegistrationResponseDto registrationResponseDto = await _authService.Register(model);
            if (!string.IsNullOrEmpty(registrationResponseDto.Message))
            {
                _response.IsSuccess = false;
                _response.Message = registrationResponseDto.Message;
                return BadRequest(_response);
            }

            string? topic_queue_Name = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
            await _serviceBus.PublishMessage(model.Email, topic_queue_Name);
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            LoginResponseDto loginResponse = await _authService.Login(model);
            if (loginResponse.User is null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            bool assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
}
