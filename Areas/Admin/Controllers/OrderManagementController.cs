using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using THweb.Models.Entities;
using THweb.Services.Interfaces;

namespace THweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderManagementController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderManagementController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(null); // Lấy tất cả đơn hàng
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id || !ModelState.IsValid) return NotFound();
            // Cập nhật trạng thái (giả sử có Status)
            var existingOrder = await _orderService.GetOrderByIdAsync(id);
            existingOrder.Status = order.Status; // Cập nhật trường Status
            await _orderService.UpdateOrderAsync(existingOrder); // Thêm phương thức UpdateOrderAsync trong IOrderService
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.DeleteOrderAsync(id); // Thêm phương thức DeleteOrderAsync trong IOrderService
            return RedirectToAction(nameof(Index));
        }
    }
}