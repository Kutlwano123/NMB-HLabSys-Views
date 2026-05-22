using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult MedicalHistory()
        {
            return View();
        }

        public IActionResult TestRequests()
        {
            return View();
        }

        public IActionResult ViewResult(int id)
        {
            ViewBag.ResultId = id;
            return View();
        }

        public IActionResult Consent()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }
    }
}