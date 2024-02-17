using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CartService : ICartService
    {
        private IMapper _mapper;
        private IProductService _productService;
        private ICouponService _couponService;
        private readonly AppDbContext _db;

        public CartService(IMapper mapper, IProductService productService, AppDbContext db, ICouponService couponService)
        {
            _mapper = mapper;
            _productService = productService;
            _db = db;
            _couponService = couponService;
        }

        public async Task<CartDto> GetCartDto(string userId)
        {
            CartHeader source = _db.CartHeaders.First(u => u.UserId == userId);
            CartDto cart = new()
            {
                CartHeader = _mapper.Map<CartHeaderDto>(source)
            };

            IQueryable<CartDetails> cartDetails = _db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId);
            cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(cartDetails);

            IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

            foreach (var item in cart.CartDetails)
            {
                item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
            }

            return cart;
        }

        public async Task ApplyCoupon(CartDto cart)
        {
            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                {
                    cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                    cart.CartHeader.Discount = coupon.DiscountAmount;
                }
            }
        }

        public async Task ApplyCouponDto(CartDto cartDto)
        {
            var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
            cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
        }

        public async Task<CartDto> UpsertCartDto(CartDto cartDto)
        {
            CartHeader? cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
            if (cartHeaderFromDb is null)
            {
                await AddNewCartHeader(cartDto);
            }
            else
            {
                await UpdateExistingCartHeader(cartDto, cartHeaderFromDb);
            }

            return cartDto;
        }

        public async Task RemoveCartDto(int cartDetailsId)
        {
            CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

            int totalCountofCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
            _db.CartDetails.Remove(cartDetails);
            if (totalCountofCartItem == 1)
            {
                var cartHeaderToRemove = await _db.CartHeaders
                   .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                _db.CartHeaders.Remove(cartHeaderToRemove);
            }
            await _db.SaveChangesAsync();
        }

        private async Task AddNewCartHeader(CartDto cartDto)
        {
            CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
            _db.CartHeaders.Add(cartHeader);
            if (cartHeader.CartHeaderId != 0)
            {
                cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                _db.CartDetails.Add(cartDetails);
            }
            await _db.SaveChangesAsync();
        }

        private async Task UpdateExistingCartHeader(CartDto cartDto, CartHeader? cartHeaderFromDb)
        {
            CartDetails? cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

            if (cartDetailsFromDb is null)
            {
                await AddNewCartDetails(cartDto, cartHeaderFromDb);
            }
            else
            {
                await UpdateExistingCartDetails(cartDto, cartDetailsFromDb);
            }
        }

        private async Task AddNewCartDetails(CartDto cartDto, CartHeader? cartHeaderFromDb)
        {
            cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
            _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
            await _db.SaveChangesAsync();
        }

        private async Task UpdateExistingCartDetails(CartDto cartDto, CartDetails? cartDetailsFromDb)
        {
            cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
            cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
            cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
            _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
            await _db.SaveChangesAsync();
        }
    }
}
