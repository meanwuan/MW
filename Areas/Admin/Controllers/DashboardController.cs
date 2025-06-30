using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace THweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            Console.WriteLine("DashboardController.Index rendered");
            return View();
        }
    }r
}