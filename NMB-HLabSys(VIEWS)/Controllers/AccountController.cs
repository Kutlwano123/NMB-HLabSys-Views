using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys_VIEWS_.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult DemoLogin(string role)
        {
            // Instantly direct mock parameters to the target layout page without real DB checking
            if (role == "Doctor")
            {
                return RedirectToAction("Dashboard", "Doctor");
            }

            // Fallback to home screen if the target panel layout isn't ready yet
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
