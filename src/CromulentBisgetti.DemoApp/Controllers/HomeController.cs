using Microsoft.AspNetCore.Mvc;

namespace CromulentBisgetti.DemoApp.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}