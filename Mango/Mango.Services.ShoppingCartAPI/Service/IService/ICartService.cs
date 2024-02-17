using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface ICartService
    {
        Task ApplyCoupon(CartDto cart);
        Task ApplyCouponDto(CartDto cartDto);
        Task<CartDto> GetCartDto(string userId);
        Task RemoveCartDto(int cartDetailsId);
        Task<CartDto> UpsertCartDto(CartDto cartDto);
    }
}