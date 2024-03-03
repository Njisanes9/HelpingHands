using Microsoft.AspNetCore.Mvc;

namespace HelpingHands.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Admin()
        {
            return View();
        }
        public IActionResult OfficeManager()
        {
            return View();
        }
        public IActionResult Nurse()
        {
            return View();
        }
        public IActionResult Patient()
        {
            return View();
        }
    }
}
