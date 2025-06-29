using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims; 
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

        
        public async Task<IActionResult> Index(int? productId)
        {
            if (!productId.HasValue)
            {
                return BadRequest("ProductId là bắt buộc.");
            }

            var product = await _productService.GetProductByIdAsync(productId.Value);
            if (product == null)
            {
                return NotFound();
            }

            var feedbacks = await _feedbackService.GetFeedbacksByProductIdAsync(productId.Value);
            var viewModel = new FeedbackViewModel
            {
                ProductId = productId.Value,
                ProductName = product.Name, 
                Comment = "",
                Rating = 0
            };

            ViewBag.Feedbacks = feedbacks;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(FeedbackViewModel model)
        {
            var product = await _productService.GetProductByIdAsync(model.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            model.ProductName = product.Name; 
            ModelState.Clear(); 
            TryValidateModel(model); 

            if (!ModelState.IsValid)
            {
                ViewBag.Feedbacks = await _feedbackService.GetFeedbacksByProductIdAsync(model.ProductId);
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Không thể xác định người dùng. Vui lòng đăng nhập lại.");
                ViewBag.Feedbacks = await _feedbackService.GetFeedbacksByProductIdAsync(model.ProductId);
                return View(model);
            }

            var feedback = new Feedback
            {
                UserId = userId,
                ProductId = model.ProductId,
                Comment = model.Comment,
                Rating = model.Rating,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _feedbackService.AddFeedbackAsync(feedback);
                TempData["SuccessMessage"] = "Phản hồi của bạn đã được gửi thành công!";
                return RedirectToAction("Index", new { productId = model.ProductId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi lưu phản hồi: {ex.Message}");
                ViewBag.Feedbacks = await _feedbackService.GetFeedbacksByProductIdAsync(model.ProductId);
                return View(model);
            }
        }
    }
}