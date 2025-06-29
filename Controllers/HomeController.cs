using Microsoft.AspNetCore.Mvc;

namespace THweb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}