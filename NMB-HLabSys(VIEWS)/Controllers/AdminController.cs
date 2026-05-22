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
        public IActionResult EditTestCategory(int id)
        {
            ViewBag.CategoryId = id;

            // prototype dummy data
            ViewBag.CategoryName = id switch
            {
                1 => "Blood Tests",
                2 => "Urine Analysis",
                3 => "Microbiology",
                _ => "Unknown Category"
            };

            return View();
        }

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
        public IActionResult EditMedication(int id)
        {
            ViewBag.MedicationId = id;
            return View();
        }
        public IActionResult EditTestType(int id)
        {
            ViewBag.TestTypeId = id;
            return View();
        }
        public IActionResult Conditions()
        {
            return View();
        }
        public IActionResult EditCondition(int id)
        {
            ViewBag.ConditionId = id;
            return View();
        }

        public IActionResult Allergies()
        {
            return View();
        }
        public IActionResult EditAllergy(int id)
        {
            ViewBag.AllergyId = id;

            // Prototype dummy data (replace later with DB)
            var allergy = id switch
            {
                1 => "Penicillin",
                2 => "Latex",
                3 => "Peanuts",
                _ => "Unknown Allergy"
            };

            var status = "Active";

            ViewBag.AllergyName = allergy;
            ViewBag.Status = status;

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