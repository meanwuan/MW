
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace THweb.Models.ViewModels
{
    public class FeedbackViewModel
    {
        public int ProductId { get; set; }
        [BindNever]
        public string ProductName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}