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
    }
}
