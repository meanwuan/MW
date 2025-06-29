using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using THweb.Data;
using THweb.Models.Entities;
using THweb.Services.Interfaces;

namespace THweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FeedbackManagementController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ApplicationDbContext _context;

        public FeedbackManagementController(IFeedbackService feedbackService, ApplicationDbContext context)
        {
            _feedbackService = feedbackService;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 5)
        {
            var feedbacks = _context.Feedbacks.Include(f => f.Product).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                feedbacks = feedbacks.Where(f => f.Comment.Contains(searchString) || f.Product.Name.Contains(searchString));
            }

            var totalItems = await feedbacks.CountAsync();
            var items = await feedbacks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Product)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null) return NotFound();
            return View(feedback);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}