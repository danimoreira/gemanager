using Microsoft.AspNetCore.Mvc;

namespace gerep_core
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
