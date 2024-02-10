﻿using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.CouponAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    //[Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ICouponService _couponService;
        private ResponseDto _response;
        private IMapper _mapper;

        public CouponAPIController(AppDbContext db, IMapper mapper, ICouponService couponService)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _couponService = couponService;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                _response.Result = _couponService.GetCouponDtos();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto GetCoupon(int id)
        {
            try
            {
                _response.Result = _couponService.GetCoupon(id);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetCouponByCode(string code)
        {
            try
            {
                _response.Result = _couponService.GetCouponByCode(code);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        //[HttpPost]
        //[Authorize(Roles = "ADMIN")]
        //public ResponseDto Post([FromBody] CouponDto couponDto)
        //{
        //    try
        //    {
        //        Coupon obj = _mapper.Map<Coupon>(couponDto);
        //        _db.Coupons.Add(obj);
        //        _db.SaveChanges();

        //        var options = new Stripe.CouponCreateOptions
        //        {
        //            AmountOff = (long)(couponDto.DiscountAmount*100),
        //            Name = couponDto.CouponCode,
        //            Currency="usd",
        //            Id=couponDto.CouponCode,
        //        };
        //        var service = new Stripe.CouponService();
        //        service.Create(options);

        //        _response.Result = _mapper.Map<CouponDto>(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.Message;
        //    }
        //    return _response;
        //}


        [HttpPut]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDto UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                _response.Result =_couponService.UpdateCoupon(couponDto);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        //[HttpDelete]
        //[Route("{id:int}")]
        //[Authorize(Roles = "ADMIN")]
        //public ResponseDto Delete(int id)
        //{
        //    try
        //    {
        //        Coupon obj = _db.Coupons.First(u=>u.CouponId==id);
        //        _db.Coupons.Remove(obj);
        //        _db.SaveChanges();


        //        var service = new Stripe.CouponService();
        //        service.Delete(obj.CouponCode);


        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.Message;
        //    }
        //    return _response;
        //}
    }
}
