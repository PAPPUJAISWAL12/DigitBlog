using Microsoft.AspNetCore.Mvc;

namespace DigitBlog.Controllers
{
    public class StaticController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Dashboard","Account");
            }
            return View();
        }
    }
}
