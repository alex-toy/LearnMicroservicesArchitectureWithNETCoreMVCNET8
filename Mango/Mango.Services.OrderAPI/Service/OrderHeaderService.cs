using AutoMapper;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Utility;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Service
{
    public class OrderHeaderService : IOrderHeaderService
    {
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IServiceBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderHeaderService(IMapper mapper, AppDbContext db, IConfiguration configuration, IServiceBus messageBus)
        {
            _mapper = mapper;
            _db = db;
            _configuration = configuration;
            _messageBus = messageBus;
        }

        public IEnumerable<OrderHeaderDto> GetOrderHeaders(string? userId, bool isAdmin)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (isAdmin)
            {
                orderHeaders = _db.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();
            }
            else
            {
                orderHeaders = _db.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();
            }
            IEnumerable<OrderHeaderDto> orderHeadersDto = _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);
            return orderHeadersDto;
        }

        public OrderHeaderDto GetOrderHeaderDto(int id)
        {
            OrderHeader orderHeader = _db.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);
            OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(orderHeader);
            return orderHeaderDto;
        }

        public async Task<OrderHeaderDto> CreateOrder(CartDto cartDto)
        {
            OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
            orderHeaderDto.OrderTime = DateTime.Now;
            orderHeaderDto.Status = SD.Status_Pending;
            orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);
            orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);
            OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
            await _db.SaveChangesAsync();

            orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
            return orderHeaderDto;
        }

        public async Task<OrderHeaderDto> GetOrderHeaderDtoAsync(int orderHeaderId)
        {
            OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

            var service = new SessionService();
            Session session = service.Get(orderHeader.StripeSessionId);

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

            if (paymentIntent.Status != "succeeded") return null;

            orderHeader.PaymentIntentId = paymentIntent.Id;
            orderHeader.Status = SD.Status_Approved;
            _db.SaveChanges();

            RewardsDto rewardsDto = new()
            {
                OrderId = orderHeader.OrderHeaderId,
                RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                UserId = orderHeader.UserId
            };

            string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            await _messageBus.PublishMessage(rewardsDto, topicName);
            OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(orderHeader);

            return orderHeaderDto;
        }

        public void SaveOrderHeader(string newStatus, int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderId);

            if (orderHeader is null) return;

            if (newStatus == SD.Status_Cancelled)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);
            }
            orderHeader.Status = newStatus;

            _db.SaveChanges();
        }

        public void SetStripeRequestDto(StripeRequestDto stripeRequestDto)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = stripeRequestDto.ApprovedUrl,
                CancelUrl = stripeRequestDto.CancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            List<SessionDiscountOptions> DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };

            options.LineItems = stripeRequestDto.OrderHeader.OrderDetails.Select(item => GetSessionLineItemOptions(item)).ToList();

            if (stripeRequestDto.OrderHeader.Discount > 0) options.Discounts = DiscountsObj;

            var service = new SessionService();
            Session session = service.Create(options);

            stripeRequestDto.StripeSessionUrl = session.Url;

            CreateOrderHeader(stripeRequestDto.OrderHeader.OrderHeaderId, session.Id);
        }

        private void CreateOrderHeader(int orderHeaderId, string sessionId)
        {
            OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);
            orderHeader.StripeSessionId = sessionId;
            _db.SaveChanges();
        }

        private static SessionLineItemOptions GetSessionLineItemOptions(OrderDetailsDto item)
        {
            SessionLineItemOptions sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name
                    }
                },
                Quantity = item.Count
            };

            return sessionLineItem;
        }
    }
}
