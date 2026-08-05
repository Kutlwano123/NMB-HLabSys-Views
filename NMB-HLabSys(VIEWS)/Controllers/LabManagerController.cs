using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys_VIEWS_.Models;

namespace NMB_HLabSys_VIEWS_.Controllers
{
    public class LabManagerController : Controller
    {
        private readonly ILabManagerService _labManagerService;

        public LabManagerController(ILabManagerService labManagerService)
        {
            _labManagerService = labManagerService;
        }

        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        public IActionResult Dashboard()
        {
            var categories = _labManagerService.GetCategories().ToList();
            var testTypes = _labManagerService.GetTestTypes().ToList();
            var consumables = _labManagerService.GetConsumables().ToList();
            var orders = _labManagerService.GetOrders().ToList();
            ViewBag.CategoryCount = categories.Count;
            ViewBag.TestTypeCount = testTypes.Count;
            ViewBag.ConsumableAlertCount = _labManagerService.GetReorderAlerts().Count();
            ViewBag.OrderCount = orders.Count;
            return View();
        }

        public IActionResult Consumables()
        {
            ViewBag.SupplierNames = _labManagerService.GetSuppliers()
                .ToDictionary(x => x.Id, x => x.SupplierName);

            return View("Consumables/Index", _labManagerService.GetConsumables().ToList());
        }

        public IActionResult ConsumableDetails(int id)
        {
            var model = _labManagerService.GetConsumable(id);
            if (model == null) return NotFound();
            ViewBag.SupplierNames = _labManagerService.GetSuppliers()
                .ToDictionary(x => x.Id, x => x.SupplierName);
            return View("Consumables/Details", model);
        }

        [HttpGet]
        public IActionResult CreateConsumable()
        {
            ViewBag.Suppliers = _labManagerService.GetSuppliers().ToList();
            return View("Consumables/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateConsumable(IFormCollection form)
        {
            try
            {
                var name = GetValue(form, "ConsumableName", "Name");
                var reorderLevel = int.Parse(GetValue(form, "ReorderLevel", "Reorder") ?? "0");
                var quantityOnHand = int.Parse(GetValue(form, "QuantityOnHand", "Quantity") ?? "0");
                var supplierId = int.Parse(GetValue(form, "SupplierId", "Supplier") ?? "0");
                _labManagerService.CreateConsumable(name, reorderLevel, quantityOnHand, supplierId);
                return RedirectToAction(nameof(Consumables));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to create consumable. Please verify your values.";
                return RedirectToAction(nameof(Consumables));
            }
        }

        [HttpGet]
        public IActionResult EditConsumable(int id)
        {
            var model = _labManagerService.GetConsumable(id);
            if (model == null) return NotFound();
            ViewBag.Suppliers = _labManagerService.GetSuppliers().ToList();
            return View("Consumables/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditConsumable(int id, IFormCollection form)
        {
            try
            {
                var name = GetValue(form, "ConsumableName", "Name");
                var reorderLevel = int.Parse(GetValue(form, "ReorderLevel", "Reorder") ?? "0");
                var quantityOnHand = int.Parse(GetValue(form, "QuantityOnHand", "Quantity") ?? "0");
                var supplierId = int.Parse(GetValue(form, "SupplierId", "Supplier") ?? "0");
                _labManagerService.UpdateConsumable(id, name, reorderLevel, quantityOnHand, supplierId);
                return RedirectToAction(nameof(Consumables));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to update consumable.";
                return RedirectToAction(nameof(Consumables));
            }
        }

        [HttpGet]
        public IActionResult DeleteConsumable(int id)
        {
            var model = _labManagerService.GetConsumable(id);
            if (model == null) return NotFound();
            return View("Consumables/Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConsumable(int id, int? consumableId)
        {
            var deleteId = consumableId ?? id;
            _labManagerService.DeleteConsumable(deleteId);
            return RedirectToAction(nameof(Consumables));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IncreaseStock(IFormCollection form)
        {
            try
            {
                var consumableId = int.Parse(GetValue(form, "ConsumableId", "ConsumableID") ?? "0");
                var adjustAmount = int.Parse(GetValue(form, "AdjustAmount", "AdjustmentValue") ?? "0");
                var success = consumableId > 0 && adjustAmount > 0 && _labManagerService.AdjustStock(consumableId, adjustAmount, "increase");
                TempData["StatusMessage"] = success ? "Stock increased successfully." : "Unable to increase stock.";
                return RedirectToAction(nameof(ConsumableDetails), new { id = consumableId });
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to increase stock.";
                return RedirectToAction(nameof(Consumables));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DecreaseStock(IFormCollection form)
        {
            try
            {
                var consumableId = int.Parse(GetValue(form, "ConsumableId", "ConsumableID") ?? "0");
                var adjustAmount = int.Parse(GetValue(form, "AdjustAmount", "AdjustmentValue") ?? "0");
                var success = consumableId > 0 && adjustAmount > 0 && _labManagerService.AdjustStock(consumableId, adjustAmount, "decrease");
                TempData["StatusMessage"] = success ? "Stock decreased successfully." : "Unable to decrease stock.";
                return RedirectToAction(nameof(ConsumableDetails), new { id = consumableId });
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to decrease stock.";
                return RedirectToAction(nameof(Consumables));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetStock(IFormCollection form)
        {
            try
            {
                var consumableId = int.Parse(GetValue(form, "ConsumableId", "ConsumableID") ?? "0");
                var newQuantity = int.Parse(GetValue(form, "NewQuantity", "NewValue") ?? "0");
                var success = consumableId > 0 && _labManagerService.AdjustStock(consumableId, newQuantity, "set");
                TempData["StatusMessage"] = success ? "Stock level updated." : "Unable to set stock level.";
                return RedirectToAction(nameof(ConsumableDetails), new { id = consumableId });
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to set stock level.";
                return RedirectToAction(nameof(Consumables));
            }
        }

        public IActionResult TestTypes()
        {
            var testTypes = _labManagerService.GetTestTypes().ToList();
            ViewBag.CategoryNames = _labManagerService.GetCategories()
                .ToDictionary(x => x.Id, x => x.CategoryName);

            return View("TestTypes/Index", testTypes);
        }

        public IActionResult TestTypeDetails(int id)
        {
            var model = _labManagerService.GetTestType(id);
            if (model == null) return NotFound();
            ViewBag.Category = _labManagerService.GetCategory(model.CategoryId);
            return View("TestTypes/Details", model);
        }

        [HttpGet]
        public IActionResult CreateTestType()
        {
            ViewBag.Categories = _labManagerService.GetCategories().ToList();
            ViewBag.Consumables = _labManagerService.GetConsumables().ToList();
            ViewBag.Technicians = _labManagerService.GetLabTechnicians().ToList();
            return View("TestTypes/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTestType(IFormCollection form)
        {
            try
            {
                var testName = GetValue(form, "TestName", "Name");
                var categoryId = int.Parse(GetValue(form, "CategoryId", "Category") ?? "0");
                var sampleType = GetValue(form, "SampleType");
                var unitOfMeasurement = GetValue(form, "UnitOfMeasurement", "Units");
                var minRange = GetValue(form, "NormalRangeMin", "MinNormalRange");
                var maxRange = GetValue(form, "NormalRangeMax", "MaxNormalRange");
                var turnaroundTime = int.Parse(GetValue(form, "TurnaroundTimeMinutes", "TurnaroundTime") ?? "0");
                var consumableIds = GetIntList(form, "ConsumableIds", "Consumables");
                var technicianIds = GetIntList(form, "TechnicianIds", "Technicians");
                _labManagerService.CreateTestType(testName, categoryId, sampleType, unitOfMeasurement, minRange, maxRange, turnaroundTime, consumableIds, technicianIds);
                return RedirectToAction(nameof(TestTypes));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to create test type.";
                return RedirectToAction(nameof(TestTypes));
            }
        }

        [HttpGet]
        public IActionResult EditTestType(int id)
        {
            var model = _labManagerService.GetTestType(id);
            if (model == null) return NotFound();
            ViewBag.Categories = _labManagerService.GetCategories().ToList();
            ViewBag.Consumables = _labManagerService.GetConsumables().ToList();
            ViewBag.Technicians = _labManagerService.GetLabTechnicians().ToList();
            return View("TestTypes/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTestType(int id, IFormCollection form)
        {
            try
            {
                var testName = GetValue(form, "TestName", "Name");
                var categoryId = int.Parse(GetValue(form, "CategoryId", "Category") ?? "0");
                var sampleType = GetValue(form, "SampleType");
                var unitOfMeasurement = GetValue(form, "UnitOfMeasurement", "Units");
                var minRange = GetValue(form, "NormalRangeMin", "MinNormalRange");
                var maxRange = GetValue(form, "NormalRangeMax", "MaxNormalRange");
                var turnaroundTime = int.Parse(GetValue(form, "TurnaroundTimeMinutes", "TurnaroundTime") ?? "0");
                var consumableIds = GetIntList(form, "ConsumableIds", "Consumables");
                var technicianIds = GetIntList(form, "TechnicianIds", "Technicians");
                _labManagerService.UpdateTestType(id, testName, categoryId, sampleType, unitOfMeasurement, minRange, maxRange, turnaroundTime, consumableIds, technicianIds);
                return RedirectToAction(nameof(TestTypes));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to update test type.";
                return RedirectToAction(nameof(TestTypes));
            }
        }

        [HttpGet]
        public IActionResult DeleteTestType(int id)
        {
            var model = _labManagerService.GetTestType(id);
            if (model == null) return NotFound();
            return View("TestTypes/Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTestType(int id, int? testTypeId)
        {
            var deleteId = testTypeId ?? id;
            _labManagerService.DeleteTestType(deleteId);
            return RedirectToAction(nameof(TestTypes));
        }

        public IActionResult TestCategories()
        {
            return View("TestCategories/Index", _labManagerService.GetCategories().ToList());
        }

        public IActionResult TestCategoryDetails(int id)
        {
            var category = _labManagerService.GetCategory(id);
            if (category == null) return NotFound();
            return View("TestCategories/Details", category);
        }

        [HttpGet]
        public IActionResult CreateTestCategory()
        {
            return View("TestCategories/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTestCategory(IFormCollection form)
        {
            try
            {
                var categoryName = GetValue(form, "CategoryName", "Name");
                var description = GetValue(form, "Description");
                _labManagerService.CreateCategory(categoryName, description);
                return RedirectToAction(nameof(TestCategories));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to create category.";
                return RedirectToAction(nameof(TestCategories));
            }
        }

        [HttpGet]
        public IActionResult EditTestCategory(int id)
        {
            var category = _labManagerService.GetCategory(id);
            if (category == null) return NotFound();
            return View("TestCategories/Edit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTestCategory(int id, IFormCollection form)
        {
            try
            {
                var categoryName = GetValue(form, "CategoryName", "Name");
                var description = GetValue(form, "Description");
                _labManagerService.UpdateCategory(id, categoryName, description);
                return RedirectToAction(nameof(TestCategories));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to update category.";
                return RedirectToAction(nameof(TestCategories));
            }
        }

        [HttpGet]
        public IActionResult DeleteTestCategory(int id)
        {
            var category = _labManagerService.GetCategory(id);
            if (category == null) return NotFound();
            ViewBag.CanDelete = _labManagerService.GetTestTypes().All(x => x.CategoryId != id);
            return View("TestCategories/Delete", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTestCategory(int id, int? testCategoryId)
        {
            var deleteId = testCategoryId ?? id;
            _labManagerService.DeleteCategory(deleteId);
            return RedirectToAction(nameof(TestCategories));
        }

        public IActionResult Suppliers()
        {
            return View("Suppliers/Index", _labManagerService.GetSuppliers().ToList());
        }

        public IActionResult SupplierDetails(int id)
        {
            var model = _labManagerService.GetSupplier(id);
            if (model == null) return NotFound();
            return View("Suppliers/Details", model);
        }

        [HttpGet]
        public IActionResult CreateSupplier()
        {
            return View("Suppliers/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSupplier(IFormCollection form)
        {
            try
            {
                var supplierName = GetValue(form, "SupplierName", "Name");
                var contactPerson = GetValue(form, "ContactPerson");
                var emailAddress = GetValue(form, "EmailAddress");
                _labManagerService.CreateSupplier(supplierName, contactPerson, emailAddress);
                return RedirectToAction(nameof(Suppliers));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to create supplier.";
                return RedirectToAction(nameof(Suppliers));
            }
        }

        [HttpGet]
        public IActionResult EditSupplier(int id)
        {
            var model = _labManagerService.GetSupplier(id);
            if (model == null) return NotFound();
            return View("Suppliers/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSupplier(int id, IFormCollection form)
        {
            try
            {
                var supplierName = GetValue(form, "SupplierName", "Name");
                var contactPerson = GetValue(form, "ContactPerson");
                var emailAddress = GetValue(form, "EmailAddress");
                _labManagerService.UpdateSupplier(id, supplierName, contactPerson, emailAddress);
                return RedirectToAction(nameof(Suppliers));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to update supplier.";
                return RedirectToAction(nameof(Suppliers));
            }
        }

        [HttpGet]
        public IActionResult DeleteSupplier(int id)
        {
            var model = _labManagerService.GetSupplier(id);
            if (model == null) return NotFound();
            return View("Suppliers/Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSupplier(int id, int? supplierId)
        {
            var deleteId = supplierId ?? id;
            _labManagerService.DeleteSupplier(deleteId);
            return RedirectToAction(nameof(Suppliers));
        }

        public IActionResult Doctors()
        {
            return View("Doctors/Index", _labManagerService.GetDoctors().ToList());
        }

        public IActionResult DoctorDetails(int id)
        {
            var model = _labManagerService.GetDoctor(id);
            if (model == null) return NotFound();
            return View("Doctors/Details", model);
        }

        [HttpGet]
        public IActionResult CreateDoctor()
        {
            return View("Doctors/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDoctor(IFormCollection form)
        {
            try
            {
                var name = GetValue(form, "Name");
                var surname = GetValue(form, "Surname");
                var hpcsaNumber = GetValue(form, "HpcsaNumber");
                var emailAddress = GetValue(form, "EmailAddress");
                var contactNumber = GetValue(form, "ContactNumber");
                _labManagerService.CreateDoctor(name, surname, hpcsaNumber, emailAddress, contactNumber);
                return RedirectToAction(nameof(Doctors));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to create doctor user.";
                return RedirectToAction(nameof(Doctors));
            }
        }

        [HttpGet]
        public IActionResult EditDoctor(int id)
        {
            var model = _labManagerService.GetDoctor(id);
            if (model == null) return NotFound();
            return View("Doctors/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDoctor(int id, IFormCollection form)
        {
            try
            {
                var name = GetValue(form, "Name");
                var surname = GetValue(form, "Surname");
                var hpcsaNumber = GetValue(form, "HpcsaNumber");
                var emailAddress = GetValue(form, "EmailAddress");
                var contactNumber = GetValue(form, "ContactNumber");
                _labManagerService.UpdateDoctor(id, name, surname, hpcsaNumber, emailAddress, contactNumber);
                return RedirectToAction(nameof(Doctors));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to update doctor user.";
                return RedirectToAction(nameof(Doctors));
            }
        }

        [HttpGet]
        public IActionResult DeleteDoctor(int id)
        {
            var model = _labManagerService.GetDoctor(id);
            if (model == null) return NotFound();
            return View("Doctors/Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDoctor(int id, int? doctorId)
        {
            var deleteId = doctorId ?? id;
            _labManagerService.DeleteDoctor(deleteId);
            return RedirectToAction(nameof(Doctors));
        }

        public IActionResult LabTechnicians()
        {
            return View("LabTechnicians/Index", _labManagerService.GetLabTechnicians().ToList());
        }

        public IActionResult LabTechnicianDetails(int id)
        {
            var model = _labManagerService.GetLabTechnician(id);
            if (model == null) return NotFound();
            ViewBag.AssignedTestTypes = _labManagerService.GetTestTypes().Where(x => model.AssignedTestTypeIds.Contains(x.Id)).ToList();
            return View("LabTechnicians/Details", model);
        }

        [HttpGet]
        public IActionResult CreateLabTechnician()
        {
            ViewBag.TestTypes = _labManagerService.GetTestTypes().ToList();
            return View("LabTechnicians/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateLabTechnician(IFormCollection form)
        {
            try
            {
                var name = GetValue(form, "Name");
                var surname = GetValue(form, "Surname");
                var southAfricanIdNumber = GetValue(form, "SouthAfricanIdNumber");
                var employeeNumber = GetValue(form, "EmployeeNumber");
                var emailAddress = GetValue(form, "EmailAddress");
                var contactNumber = GetValue(form, "ContactNumber");
                var assignedTestTypeIds = GetIntList(form, "AssignedTestTypeIds", "TestTypes");
                _labManagerService.CreateLabTechnician(name, surname, southAfricanIdNumber, employeeNumber, emailAddress, contactNumber, assignedTestTypeIds);
                return RedirectToAction(nameof(LabTechnicians));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to create technician.";
                return RedirectToAction(nameof(LabTechnicians));
            }
        }

        [HttpGet]
        public IActionResult EditLabTechnician(int id)
        {
            var model = _labManagerService.GetLabTechnician(id);
            if (model == null) return NotFound();
            ViewBag.TestTypes = _labManagerService.GetTestTypes().ToList();
            return View("LabTechnicians/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLabTechnician(int id, IFormCollection form)
        {
            try
            {
                var name = GetValue(form, "Name");
                var surname = GetValue(form, "Surname");
                var southAfricanIdNumber = GetValue(form, "SouthAfricanIdNumber");
                var employeeNumber = GetValue(form, "EmployeeNumber");
                var emailAddress = GetValue(form, "EmailAddress");
                var contactNumber = GetValue(form, "ContactNumber");
                var assignedTestTypeIds = GetIntList(form, "AssignedTestTypeIds", "TestTypes");
                _labManagerService.UpdateLabTechnician(id, name, surname, southAfricanIdNumber, employeeNumber, emailAddress, contactNumber, assignedTestTypeIds);
                return RedirectToAction(nameof(LabTechnicians));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to update technician.";
                return RedirectToAction(nameof(LabTechnicians));
            }
        }

        [HttpGet]
        public IActionResult DeleteLabTechnician(int id)
        {
            var model = _labManagerService.GetLabTechnician(id);
            if (model == null) return NotFound();
            return View("LabTechnicians/Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteLabTechnician(int id, int? technicianId)
        {
            var deleteId = technicianId ?? id;
            _labManagerService.DeleteLabTechnician(deleteId);
            return RedirectToAction(nameof(LabTechnicians));
        }

        public IActionResult SampleTypes()
        {
            return View("SampleTypes/Index", _labManagerService.GetSampleTypes().ToList());
        }

        public IActionResult SampleTypeDetails(int id)
        {
            var model = _labManagerService.GetSampleType(id);
            if (model == null) return NotFound();
            return View("SampleTypes/Details", model);
        }

        [HttpGet]
        public IActionResult CreateSampleType()
        {
            return View("SampleTypes/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSampleType(IFormCollection form)
        {
            try
            {
                var name = GetValue(form, "Name", "SampleTypeName");
                var description = GetValue(form, "Description");
                _labManagerService.CreateSampleType(name, description);
                return RedirectToAction(nameof(SampleTypes));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to create sample type.";
                return RedirectToAction(nameof(SampleTypes));
            }
        }

        [HttpGet]
        public IActionResult EditSampleType(int id)
        {
            var model = _labManagerService.GetSampleType(id);
            if (model == null) return NotFound();
            return View("SampleTypes/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSampleType(int id, IFormCollection form)
        {
            try
            {
                var name = GetValue(form, "Name", "SampleTypeName");
                var description = GetValue(form, "Description");
                _labManagerService.UpdateSampleType(id, name, description);
                return RedirectToAction(nameof(SampleTypes));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to update sample type.";
                return RedirectToAction(nameof(SampleTypes));
            }
        }

        [HttpGet]
        public IActionResult DeleteSampleType(int id)
        {
            var model = _labManagerService.GetSampleType(id);
            if (model == null) return NotFound();
            return View("SampleTypes/Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSampleType(int id, int? sampleTypeId)
        {
            var deleteId = sampleTypeId ?? id;
            _labManagerService.DeleteSampleType(deleteId);
            return RedirectToAction(nameof(SampleTypes));
        }

        public IActionResult Orders()
        {
            ViewBag.SupplierNames = _labManagerService.GetSuppliers()
                .ToDictionary(x => x.Id, x => x.SupplierName);
            return View("Orders/Index", _labManagerService.GetOrders().ToList());
        }

        public IActionResult OrderDetails(int id)
        {
            var model = _labManagerService.GetOrder(id);
            if (model == null) return NotFound();
            ViewBag.Supplier = _labManagerService.GetSupplier(model.SupplierId);
            ViewBag.ConsumableNames = _labManagerService.GetConsumables()
                .ToDictionary(x => x.Id, x => x.ConsumableName);
            return View("Orders/Details", model);
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            ViewBag.Suppliers = _labManagerService.GetSuppliers().ToList();
            ViewBag.Consumables = _labManagerService.GetConsumables().ToList();
            return View("Orders/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrder(IFormCollection form)
        {
            try
            {
                var supplierId = int.Parse(GetValue(form, "SupplierId", "Supplier") ?? "0");
                var orderNumber = GetValue(form, "OrderNumber");
                var items = ParseOrderItems(form);
                if (supplierId <= 0 || !items.Any())
                {
                    throw new InvalidOperationException("Please select a supplier and at least one consumable item.");
                }
                _labManagerService.CreateOrder(supplierId, items, orderNumber);
                TempData["StatusMessage"] = "Order created successfully.";
                return RedirectToAction(nameof(Orders));
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Unable to create order.";
                return RedirectToAction(nameof(Orders));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOrder(int id)
        {
            _labManagerService.DeleteOrder(id);
            TempData["StatusMessage"] = "Order deleted.";
            return RedirectToAction(nameof(Orders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReceiveOrder(int id)
        {
            _labManagerService.ReceiveOrder(id);
            TempData["StatusMessage"] = "Order marked as received.";
            return RedirectToAction(nameof(Orders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(int id, string reason)
        {
            var cancellationReason = string.IsNullOrWhiteSpace(reason) ? "Cancelled by laboratory manager." : reason;
            _labManagerService.CancelOrder(id, cancellationReason);
            TempData["StatusMessage"] = "Order cancelled.";
            return RedirectToAction(nameof(Orders));
        }

        [HttpGet]
        public IActionResult ReorderAlerts()
        {
            return View("Orders/ReorderAlerts", _labManagerService.GetReorderAlerts().ToList());
        }

        public IActionResult Reports(DateTime? startDate, DateTime? endDate)
        {
            var model = new ReportsViewModel
            {
                StartDate = startDate ?? DateTime.Today.AddDays(-30),
                EndDate = endDate ?? DateTime.Today,
                ReportRows = _labManagerService.GetReports(startDate ?? DateTime.Today.AddDays(-30), endDate ?? DateTime.Today).ToList()
            };
            return View("Reports", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reports(ReportsViewModel model)
        {
            model.ReportRows = _labManagerService.GetReports(model.StartDate, model.EndDate).ToList();
            return View("Reports", model);
        }

        [HttpGet]
        public IActionResult ExportReportPdf(DateTime startDate, DateTime endDate)
        {
            var pdf = _labManagerService.GenerateReportPdf(startDate, endDate);
            return File(System.Text.Encoding.UTF8.GetBytes(pdf), "application/pdf", $"lab-manager-report-{startDate:yyyyMMdd}-{endDate:yyyyMMdd}.pdf");
        }

        public IActionResult Profile()
        {
            return View("Profile");
        }

        private List<(int ConsumableId, int Quantity)> ParseOrderItems(IFormCollection form)
        {
            var rawItems = GetValue(form, "OrderItems", "Items");
            if (!string.IsNullOrWhiteSpace(rawItems))
            {
                return rawItems.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Split(':', StringSplitOptions.TrimEntries))
                    .Where(parts => parts.Length >= 2 && int.TryParse(parts[0], out _) && int.TryParse(parts[1], out _))
                    .Select(parts => (int.Parse(parts[0]), int.Parse(parts[1])))
                    .Where(item => item.Item1 > 0 && item.Item2 > 0)
                    .ToList();
            }

            var consumableIds = GetIntList(form, "ConsumableIds", "Consumables");
            var quantities = GetIntList(form, "Quantities", "Quantity");
            return consumableIds.Zip(quantities.DefaultIfEmpty(0), (consumableId, quantity) => (consumableId, quantity))
                .Where(item => item.consumableId > 0 && item.quantity > 0)
                .ToList();
        }

        private static string GetValue(IFormCollection form, params string[] keys)
        {
            foreach (var key in keys)
            {
                var value = form[key].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }
            return string.Empty;
        }

        private static List<int> GetIntList(IFormCollection form, params string[] keys)
        {
            foreach (var key in keys)
            {
                var values = form[key];
                if (values.Any())
                {
                    return values.Where(v => int.TryParse(v, out _)).Select(int.Parse).ToList();
                }
            }
            return new List<int>();
        }
    }
}
