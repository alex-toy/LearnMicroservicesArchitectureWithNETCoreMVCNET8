using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private ICartService _cartService;
        private IConfiguration _configuration;
        private readonly IServiceBus _serviceBus;

        public CartAPIController(IServiceBus messageBus, IConfiguration configuration, ICartService cartService)
        {
            _serviceBus = messageBus;
            _response = new ResponseDto();
            _configuration = configuration;
            _cartService = cartService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = await _cartService.GetCartDto(userId);
                await _cartService.ApplyCoupon(cart);

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                await _cartService.ApplyCouponDto(cartDto);
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                string? topic_queue_Name = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
                await _serviceBus.PublishMessage(cartDto, topic_queue_Name);
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                _response.Result = await _cartService.UpsertCartDto(cartDto);
            }
            catch (Exception ex)
            {
                _response.Message= ex.Message.ToString();
                _response.IsSuccess= false;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
                await _cartService.RemoveCartDto(cartDetailsId);

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
