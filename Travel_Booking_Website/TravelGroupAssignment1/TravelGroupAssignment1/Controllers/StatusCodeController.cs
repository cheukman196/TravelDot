using Microsoft.AspNetCore.Mvc;

namespace TravelGroupAssignment1.Controllers
{
    public class StatusCodeController : Controller
    {
        public IActionResult Index(int code)
        {
            if (code == 404)
            {
                return View("404");
            }
            else if (code == 500)
            {
                return View("500");
            }
            else if (code == 502)
            {
                return View("502");
            }
            else
            {
                return View("Error");
            }
        }
    }

}
