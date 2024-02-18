using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Service;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private IOrderHeaderService _orderHeaderService;

        public OrderAPIController(IOrderHeaderService orderHeaderService)
        {
            _response = new ResponseDto();
            _orderHeaderService = orderHeaderService;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDto? Get(string? userId = "")
        {
            try
            {
                bool isAdmin = User.IsInRole(SD.RoleAdmin);
                _response.Result = _orderHeaderService.GetOrderHeaders(userId, isAdmin);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {
                _response.Result = _orderHeaderService.GetOrderHeaderDto(id);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                _response.Result = await _orderHeaderService.CreateOrder(cartDto);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message=ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                _orderHeaderService.SetStripeRequestDto(stripeRequestDto);
                _response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _response.Message= ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                _response.Result = await _orderHeaderService.GetOrderHeaderDtoAsync(orderHeaderId);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                _orderHeaderService.SaveOrderHeader(newStatus, orderId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
            }

            return _response;
        }
    }
}
