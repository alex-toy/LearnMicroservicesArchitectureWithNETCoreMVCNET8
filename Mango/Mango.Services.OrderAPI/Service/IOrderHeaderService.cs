using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI.Service
{
    public interface IOrderHeaderService
    {
        Task<OrderHeaderDto> CreateOrder(CartDto cartDto);
        OrderHeaderDto GetOrderHeaderDto(int id);
        Task<OrderHeaderDto> GetOrderHeaderDtoAsync(int orderHeaderId);
        IEnumerable<OrderHeaderDto> GetOrderHeaders(string? userId, bool isAdmin);
        void SaveOrderHeader(string newStatus, int orderId);
        void SetStripeRequestDto(StripeRequestDto stripeRequestDto);
    }
}