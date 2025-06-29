using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using THweb.Services.Interfaces;

namespace THweb.Controllers
{
    [Authorize]
    public class OrderHistoryController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderHistoryController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(User.Identity.Name);
            return View(orders);
        }
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserId != User.Identity.Name) return NotFound();
            return View(order);
        }
    }
}