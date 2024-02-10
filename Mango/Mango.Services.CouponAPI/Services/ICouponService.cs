using Mango.Services.CouponAPI.Models.Dto;

namespace Mango.Services.CouponAPI.Services
{
    public interface ICouponService
    {
        CouponDto GetCoupon(int id);
        CouponDto GetCouponByCode(string code);
        IEnumerable<CouponDto> GetCouponDtos();
        CouponDto UpdateCoupon(CouponDto couponDto);
    }
}