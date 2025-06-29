using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using THweb.Data;
using THweb.Models.Entities;
using THweb.Services.Interfaces;

namespace THweb.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;

        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("Không thể lưu phản hồi vào cơ sở dữ liệu.");
            }
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByProductIdAsync(int productId)
        {
            return await _context.Feedbacks
                .Include(f => f.Product)
                .Where(f => f.ProductId == productId)
                .ToListAsync();
        }
    }
}