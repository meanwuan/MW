using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using THweb.Models.Entities;
using THweb.Models.ViewModels;
using THweb.Services.Interfaces;

namespace THweb.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IProductService _productService;

        public FeedbackController(IFeedbackService feedbackService, IProductService productService)
        {
            _feedbackService = feedbackService;
            _productService = productService;
        }

        // Hiển thị form phản hồi và danh sách phản hồi
        public async Task<IActionResult> Index(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound();

            var feedbacks = await _feedbackService.GetFeedbacksByProductIdAsync(productId);
            var viewModel = new FeedbackViewModel
            {
                ProductId = productId,
                ProductName = product.Name,
                Comment = "",
                Rating = 0
            };
            ViewBag.Feedbacks = feedbacks;
            return View(viewModel);
        }

        // Gửi phản hồi
        [HttpPost]
        public async Task<IActionResult> Index(FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var product = await _productService.GetProductByIdAsync(model.ProductId);
                model.ProductName = product?.Name;
                return View(model);
            }

            var feedback = new Feedback
            {
                UserId = User.Identity.Name,
                ProductId = model.ProductId,
                Comment = model.Comment,
                Rating = model.Rating
            };
            await _feedbackService.AddFeedbackAsync(feedback);
            return RedirectToAction(nameof(Index), new { productId = model.ProductId });
        }
    }
}