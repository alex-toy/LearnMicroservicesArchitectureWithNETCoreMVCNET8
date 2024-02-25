using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        public IActionResult OrderIndex()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
            string userId = GetUserIdFromClaims();

            ResponseDto? response = await _orderService.GetOrder(orderId);

            if (response is not null && response.IsSuccess)
            {
                string? value = Convert.ToString(response.Result) ?? string.Empty;
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(value) ?? new OrderHeaderDto { UserId = "" };
            }

            if (!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }

            return View(orderHeaderDto);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            ResponseDto? response = await _orderService.UpdateOrderStatus(orderId, SD.ReadyForPickup);

            if (response is null || !response.IsSuccess) return View();

            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);

            if (response is null || !response.IsSuccess) return View();

            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            ResponseDto? response = await _orderService.UpdateOrderStatus(orderId, SD.Cancelled);

            if (response is null || !response.IsSuccess) return View();

            TempData["success"] = "Status updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }


        [HttpGet]
        public IActionResult GetAll(string status)
        {
            string userId = "";
            if (!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value ?? string.Empty;
            }
            ResponseDto response = _orderService.GetAllOrder(userId).GetAwaiter().GetResult();

            bool isResponseOk = response is not null && response.IsSuccess;
            IEnumerable<OrderHeaderDto> list = isResponseOk ? GetListFromStatus(status, response) : new List<OrderHeaderDto>();

            return Json(new { data = list.OrderByDescending(u => u.OrderHeaderId) });
        }

        private static IEnumerable<OrderHeaderDto> GetListFromStatus(string status, ResponseDto response)
        {
            string? value = Convert.ToString(response.Result);
            IEnumerable<OrderHeaderDto> list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(value) ?? new List<OrderHeaderDto>();

            list = status switch
            {
                "approved" => list.Where(u => u.Status == SD.Approved),
                "readyforpickup" => list.Where(u => u.Status == SD.ReadyForPickup),
                "cancelled" => list.Where(u => u.Status == SD.Cancelled || u.Status == SD.Refunded)
            };

            return list;
        }

        private string GetUserIdFromClaims()
        {
            return User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value ?? string.Empty;
        }
    }
}
