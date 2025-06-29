using System.Collections.Generic;
using System.Threading.Tasks;
using THweb.Models.Entities;

namespace THweb.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task AddFeedbackAsync(Feedback feedback);
        Task<IEnumerable<Feedback>> GetFeedbacksByProductIdAsync(int productId);
    }
}