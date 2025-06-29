using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using THweb.Models.Entities;
using THweb.Models.ViewModels;
using THweb.Services.Interfaces;

namespace THweb.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CheckoutController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCartByUserIdAsync(User.Identity.Name);
            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            var viewModel = new CheckoutViewModel
            {
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cart = await _cartService.GetCartByUserIdAsync(User.Identity.Name);
            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            var order = new Order
            {
                UserId = User.Identity.Name,
                ShippingAddress = model.ShippingAddress,
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
            };

            try
            {
                var (success, message) = await _orderService.CreateOrderAsync(order, cart.CartItems.ToList());
                if (!success)
                {
                    TempData["ErrorMessage"] = message;
                    return View(model);
                }
                await _cartService.ClearCartAsync(User.Identity.Name);
                return RedirectToAction("Index", "OrderHistory");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }
    }
}