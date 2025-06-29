using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using THweb.Models.ViewModels;
using THweb.Services.Interfaces;

namespace THweb.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCartByUserIdAsync(User.Identity.Name);
            var viewModel = new CartViewModel
            {
                CartId = cart?.Id ?? 0,
                Items = cart?.CartItems.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    ImageUrl = ci.Product.ImageUrl
                }).ToList() ?? new List<CartItemViewModel>(),
                TotalPrice = cart?.CartItems.Sum(ci => ci.Quantity * ci.Product.Price) ?? 0
            };
            ViewBag.UniqueProductCount = cart?.CartItems.Select(ci => ci.ProductId).Distinct().Count() ?? 0;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            await _cartService.AddToCartAsync(User.Identity.Name, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int productId, int quantity)
        {
            await _cartService.UpdateCartItemAsync(User.Identity.Name, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            await _cartService.RemoveFromCartAsync(User.Identity.Name, productId);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetUniqueProductCount()
        {
            var userId = User?.Identity?.Name;
            if (userId == null) return Json(0);

            var cart = await _cartService.GetCartByUserIdAsync(userId);
            var uniqueProductCount = cart?.CartItems?.Select(ci => ci.ProductId).Distinct().Count() ?? 0;
            return Json(uniqueProductCount);
        }
    }
}