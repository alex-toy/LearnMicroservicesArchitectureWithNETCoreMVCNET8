using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;

namespace Mango.Services.CouponAPI.Services
{
    public class CouponService : ICouponService
    {
        private IMapper _mapper;
        private readonly AppDbContext _db;

        public CouponService(IMapper mapper, AppDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public IEnumerable<CouponDto> GetCouponDtos()
        {
            IEnumerable<Coupon> objList = _db.Coupons.ToList();
            IEnumerable<CouponDto> couponDtos = _mapper.Map<IEnumerable<CouponDto>>(objList);
            return couponDtos;
        }

        public CouponDto GetCoupon(int id)
        {
            Coupon obj = _db.Coupons.First(u => u.CouponId == id);
            CouponDto couponDto = _mapper.Map<CouponDto>(obj);
            return couponDto;
        }

        public CouponDto GetCouponByCode(string code)
        {
            Coupon coupon = _db.Coupons.First(u => u.CouponCode.ToLower() == code.ToLower());
            CouponDto couponDto = _mapper.Map<CouponDto>(coupon);
            return couponDto;
        }

        public CouponDto UpdateCoupon(CouponDto couponDto)
        {
            Coupon coupon = _mapper.Map<Coupon>(couponDto);
            _db.Coupons.Update(coupon);
            _db.SaveChanges();

            CouponDto updatedCouponDto = _mapper.Map<CouponDto>(coupon);
            return updatedCouponDto;
        }
    }
}
