using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys.Controllers
{
  
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult EditUser(int id)
        {
            ViewBag.UserId = id;
            return View();
        }

        //public IActionResult Doctors()
        //{
        //    return View();
        //}

        //public IActionResult Technicians()
        //{
        //    return View();
        //}

        //public IActionResult Patients()
        //{
        //    return View();
        //}

        public IActionResult TestRequests()
        {
            return View();
        }

        public IActionResult TestCategories()
        {
            return View();
        }

        public IActionResult TestTypes()
        {
            return View();
        }

        public IActionResult Medications()
        {
            return View();
        }

        public IActionResult Conditions()
        {
            return View();
        }

        public IActionResult Allergies()
        {
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult AuditLog()
        {
            return View();
        }
    }
}