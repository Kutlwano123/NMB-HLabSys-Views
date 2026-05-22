using Microsoft.AspNetCore.Mvc;

namespace NMB_HLabSys_VIEWS_.Controllers
{
    public class LabManagerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Consumables()
        {
            return View("Consumables/Index");
        }
        public IActionResult ConsumableDetails()
        {
            return View("Consumables/Details");
        }
        public IActionResult CreateConsumable()
        {
            return View("Consumables/Create");
        }
        public IActionResult EditConsumable(int id)
        {
            ViewBag.ConsumableId = id;
            return View("Consumables/Edit");
        }
        public IActionResult DeleteConsumable(int id)
        {
            ViewBag.ConsumableId = id;
            return View("Consumables/Delete");
        }
        public IActionResult TestTypes()
        {
            return View("TestTypes/Index");
        }
        public IActionResult TestTypeDetails()
        {
            return View("TestTypes/Details");
        }
        public IActionResult CreateTestType()
        {
            return View("TestTypes/Create");
        }
        public IActionResult EditTestType()
        {
            return View("TestTypes/Edit");
        }
        public IActionResult DeleteTestType()
        {
            return View("TestTypes/Delete");
        }
        public IActionResult TestCategories()
        {
            return View("TestCategories/Index");
        }
        public IActionResult TestCategoryDetails()
        {
            return View("TestCategories/Details");
        }
        public IActionResult DeleteTestCategory()
        {
            return View("TestCategories/Delete");
        }
        public IActionResult CreateTestCategory()
        {
            return View("TestCategories/Create");
        }
        public IActionResult EditTestCategory()
        {
            return View("TestCategories/Edit");
        }
        public IActionResult Suppliers()
        {
            return View("Suppliers/Index");
        }
        public IActionResult CreateSupplier()
        {
            return View("Suppliers/Create");
        }
        public IActionResult EditSupplier()
        {
            return View("Suppliers/Edit");
        }
        public IActionResult DeleteSupplier()
        {
            return View("Suppliers/Delete");
        }
        public IActionResult SupplierDetails()
        {
            return View("Suppliers/Details");
        }
        public IActionResult Doctors()
        {
            return View("Doctors/Index");
        }
        public IActionResult CreateDoctor()
        {
            return View("Doctors/Create");
        }
        public IActionResult EditDoctor()
        {
            return View("Doctors/Edit");
        }
        public IActionResult DeleteDoctor()
        {
            return View("Doctors/Delete");
        }
        public IActionResult DoctorDetails()
        {
            return View("Doctors/Details");
        }
        public IActionResult LabTechnicians()
        {
            return View("LabTechnicians/Index");
        }
        public IActionResult CreateLabTechnician()
        {
            return View("LabTechnicians/Create");
        }
        public IActionResult EditLabTechnician()
        {
            return View("LabTechnicians/Edit");
        }
        public IActionResult DeleteLabTechnician()
        {
            return View("LabTechnicians/Delete");
        }
        public IActionResult LabTechnicianDetails()
        {
            return View("LabTechnicians/Details");
        }
        public IActionResult SampleTypes()
        {
            return View("SampleTypes/Index");
        }
        public IActionResult CreateSampleType()
        {
            return View("SampleTypes/Create");
        }

        public IActionResult EditSampleType()
        {
            return View("SampleTypes/Edit");
        }
        public IActionResult DeleteSampleType()
        {
            return View("SampleTypes/Delete");
        }
        public IActionResult SampleTypeDetails()
        {
            return View("SampleTypes/Details");
        }
        public IActionResult Orders()
        {
            return View("Orders/Index");
        }
        public IActionResult CreateOrder()
        {
            return View("Orders/Create");
        }
        public IActionResult EditOrder()
        {
            return View("Orders/Edit");
        }
        public IActionResult DeleteOrder()
        {
            return View("Orders/Delete");
        }
        public IActionResult OrderDetails()
        {
            return View("Orders/Details");
        }
    }

}
