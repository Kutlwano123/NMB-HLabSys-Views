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
        public IActionResult EditConsumable(IFormCollection form)
        {
            try
            {
                // Safely extract all values matching the exact 'name' attributes in our Razor view
                var id = int.TryParse(form["ConsumableId"], out var parsedId) ? parsedId : 0;
                var name = form["ConsumableName"].ToString();
                var supplierId = int.TryParse(form["SupplierId"], out var parsedSupplier) ? parsedSupplier : 0;
                var reorderLevel = int.TryParse(form["ReorderLevel"], out var parsedReorder) ? parsedReorder : 0;
                var quantityOnHand = int.TryParse(form["QuantityOnHand"], out var parsedQty) ? parsedQty : 0;

                // Pass them to your service to save
                _labManagerService.UpdateConsumable(id, name, reorderLevel, quantityOnHand, supplierId);

                // Flash a success message and redirect
                TempData["StatusMessage"] = "Consumable updated successfully.";
                return RedirectToAction(nameof(Consumables));
            }
            catch (Exception ex)
            {
                var receivedId = form["SupplierId"].ToString();

                TempData["StatusMessage"] = $"Failed: {ex.Message} (The form sent Supplier ID: '{receivedId}')";
                return RedirectToAction(nameof(Consumables));
            }
        }

        [HttpGet]
        public IActionResult DeleteConsumable(int id)
        {
            var consumable = _labManagerService.GetConsumable(id);
            if (consumable == null)
            {
                return NotFound();
            }

            var supplier = _labManagerService.GetSupplier(consumable.SupplierId);

            var viewModel = new NMB_HLabSys_VIEWS.ViewModels.DeleteConsumableViewModel
            {
                Id = consumable.Id,
                ConsumableName = consumable.ConsumableName,
                SupplierName = supplier?.SupplierName ?? "Unknown Supplier",
                ReorderLevel = consumable.ReorderLevel,
                QuantityOnHand = consumable.QuantityOnHand
            };
            return View("Consumables/Delete", viewModel);
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

        [HttpGet]
        public IActionResult TestTypes()
        {
            var testTypes = _labManagerService.GetTestTypes();
            var categories = _labManagerService.GetCategories();
            var sampleTypes = _labManagerService.GetSampleTypes();

            var viewModels = testTypes.Select(t => new NMB_HLabSys_VIEWS.ViewModels.TestTypeDetailsViewModel
            {
                Id = t.Id,
                TestName = t.TestName,
                CategoryName = categories.FirstOrDefault(c => c.Id == t.CategoryId)?.CategoryName ?? "Unknown",
                SampleTypeName = sampleTypes.FirstOrDefault(s => s.Id == t.SampleTypeId)?.Name ?? "Unknown",
                UnitOfMeasurement = t.UnitOfMeasurement,
                NormalRangeMin = t.NormalRangeMin,
                NormalRangeMax = t.NormalRangeMax,
                TurnaroundTimeMinutes = t.TurnaroundTimeMinutes
            }).ToList();

            return View("TestTypes/Index", viewModels);
        }

        public IActionResult TestTypeDetails(int id)
        {
            var model = _labManagerService.GetTestType(id);
            if (model == null) return NotFound();
            var category = _labManagerService.GetCategory(model.CategoryId);
            var sampleType = _labManagerService.GetSampleType(model.SampleTypeId);
            var consumables = _labManagerService.GetConsumables()
                .Where(c => model.ConsumableIds.Contains(c.Id))
                .ToList();
            ViewBag.Consumables = consumables;
            var technicians = _labManagerService.GetLabTechnicians()
                .Where(t => model.TechnicianIds.Contains(t.Id))
                .ToList();
            ViewBag.Technicians = technicians;
            var viewModel = new NMB_HLabSys_VIEWS.ViewModels.TestTypeDetailsViewModel
            {
                Id = model.Id,
                TestName = model.TestName,
                CategoryName = category?.CategoryName ?? "Unknown",
                SampleTypeName = sampleType?.Name ?? "Unknown",
                UnitOfMeasurement = model.UnitOfMeasurement,
                NormalRangeMin = model.NormalRangeMin,
                NormalRangeMax = model.NormalRangeMax,
                TurnaroundTimeMinutes = model.TurnaroundTimeMinutes,
                ConsumableNames = consumables.Select(c => c.ConsumableName).ToList(),
                TechnicianNames = technicians.Select(t => $"{t.Name} {t.Surname}").ToList()
            };
            return View("TestTypes/Details", viewModel);
        }

        [HttpGet]
        public IActionResult CreateTestType()
        {
            ViewBag.Categories = _labManagerService.GetCategories().ToList();
            ViewBag.SampleTypes = _labManagerService.GetSampleTypes().ToList();
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
                // Safe parsing of standard fields
                var testName = form["TestName"].ToString();
                var categoryId = int.TryParse(form["CategoryId"], out var parsedCat) ? parsedCat : 0;
                var sampleTypeId = int.TryParse(form["SampleTypeId"], out var parsedSample) ? parsedSample : 0;
                var unitOfMeasurement = form["UnitOfMeasurement"].ToString();
                var minRange = decimal.TryParse(form["NormalRangeMin"], out var parsedMin) ? parsedMin : 0m;
                var maxRange = decimal.TryParse(form["NormalRangeMax"], out var parsedMax) ? parsedMax : 0m;
                var turnaroundTime = int.TryParse(form["TurnaroundTimeMinutes"], out var parsedTime) ? parsedTime : 0;

                // Bulletproof Checkbox Parsing for Consumables
                var consumableIds = new List<int>();
                if (!string.IsNullOrWhiteSpace(form["ConsumableIds"]))
                {
                    foreach (var val in form["ConsumableIds"].ToString().Split(','))
                    {
                        if (int.TryParse(val, out var parsedCons)) consumableIds.Add(parsedCons);
                    }
                }

                // Bulletproof Checkbox Parsing for Technicians
                var technicianIds = new List<int>();
                if (!string.IsNullOrWhiteSpace(form["TechnicianIds"]))
                {
                    foreach (var val in form["TechnicianIds"].ToString().Split(','))
                    {
                        if (int.TryParse(val, out var parsedTech)) technicianIds.Add(parsedTech);
                    }
                }

                // Pass to your Service
                _labManagerService.CreateTestType(
                    testName, categoryId, sampleTypeId, unitOfMeasurement,
                    minRange, maxRange, turnaroundTime, consumableIds, technicianIds
                );

                TempData["StatusMessage"] = "Test type successfully created.";
                return RedirectToAction(nameof(TestTypes));
            }
            catch (Exception ex)
            {
                // If it fails (like a duplicate name), output exactly why
                TempData["StatusMessage"] = $"Unable to create test type. Error: {ex.Message}";
                return RedirectToAction(nameof(TestTypes));
            }
        }

        [HttpGet]
        public IActionResult EditTestType(int id)
        {
            var testType = _labManagerService.GetTestType(id);
            if (testType == null) return NotFound();

            var allConsumables = _labManagerService.GetConsumables();
            var consumableCheckboxes = allConsumables.Select(c => new NMB_HLabSys_VIEWS.ViewModels.CheckboxItemViewModel
            {
                Id = c.Id,
                Name = c.ConsumableName,
                IsSelected = testType.ConsumableIds.Contains(c.Id)
            }).ToList();

            var allTechnicians = _labManagerService.GetLabTechnicians();
            var technicianCheckboxes = allTechnicians.Select(t => new NMB_HLabSys_VIEWS.ViewModels.CheckboxItemViewModel
            {
                Id = t.Id,
                Name = $"{t.Name} {t.Surname}",
                IsSelected = testType.TechnicianIds.Contains(t.Id)
            }).ToList();

            var viewModel = new NMB_HLabSys_VIEWS.ViewModels.EditTestTypeViewModel
            {
                Id = testType.Id,
                TestName = testType.TestName,
                CategoryId = testType.CategoryId,
                SampleTypeId = testType.SampleTypeId,
                UnitOfMeasurement = testType.UnitOfMeasurement,
                NormalRangeMin = testType.NormalRangeMin,
                NormalRangeMax = testType.NormalRangeMax,
                TurnaroundTimeMinutes = testType.TurnaroundTimeMinutes,
                AvailableCategories = _labManagerService.GetCategories().ToList(),
                AvailableSampleTypes = _labManagerService.GetSampleTypes().ToList(),
                AvailableConsumables = consumableCheckboxes,
                AvailableTechnicians = technicianCheckboxes
            };

            return View("TestTypes/Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTestType(IFormCollection form)
        {
            try
            {
                // Safe parsing: defaults to 0 instead of crashing if the form data is missing
                var id = int.TryParse(form["TestTypeID"], out var parsedId) ? parsedId : 0;
                var testName = form["TestName"].ToString();
                var categoryId = int.TryParse(form["CategoryId"], out var parsedCat) ? parsedCat : 0;
                var sampleTypeId = int.TryParse(form["SampleTypeId"], out var parsedSample) ? parsedSample : 0;
                var unitOfMeasurement = form["UnitOfMeasurement"].ToString();
                var minRange = decimal.TryParse(form["NormalRangeMin"], out var parsedMin) ? parsedMin : 0m;
                var maxRange = decimal.TryParse(form["NormalRangeMax"], out var parsedMax) ? parsedMax : 0m;
                var turnaroundTime = int.TryParse(form["TurnaroundTimeMinutes"], out var parsedTime) ? parsedTime : 0;

                // Bulletproof Checkbox Parsing
                var consumableIds = new List<int>();
                if (!string.IsNullOrWhiteSpace(form["ConsumableIds"]))
                {
                    foreach (var val in form["ConsumableIds"].ToString().Split(','))
                    {
                        if (int.TryParse(val, out var parsedCons)) consumableIds.Add(parsedCons);
                    }
                }

                var technicianIds = new List<int>();
                if (!string.IsNullOrWhiteSpace(form["TechnicianIds"]))
                {
                    foreach (var val in form["TechnicianIds"].ToString().Split(','))
                    {
                        if (int.TryParse(val, out var parsedTech)) technicianIds.Add(parsedTech);
                    }
                }

                _labManagerService.UpdateTestType(
                    id, testName, categoryId, sampleTypeId, unitOfMeasurement,
                    minRange, maxRange, turnaroundTime, consumableIds, technicianIds
                );

                return RedirectToAction(nameof(TestTypes));
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"Unable to update test type. Error: {ex.Message}";
                return RedirectToAction(nameof(TestTypes));
            }
        }

        [HttpGet]
        public IActionResult DeleteTestType(int id)
        {
            var testType = _labManagerService.GetTestType(id);
            if (testType == null)
            {
                return NotFound();
            }

            var category = _labManagerService.GetCategory(testType.CategoryId);
            var sampleType = _labManagerService.GetSampleType(testType.SampleTypeId);

            var viewModel = new NMB_HLabSys_VIEWS.ViewModels.DeleteTestTypeViewModel
            {
                Id = testType.Id,
                TestName = testType.TestName,
                CategoryName = category?.CategoryName ?? "Unknown",
                SampleTypeName = sampleType?.Name ?? "Unknown",
                TurnaroundTimeMinutes = testType.TurnaroundTimeMinutes
            };

            return View("TestTypes/Delete", viewModel);
        }

        [HttpPost, ActionName("DeleteTestType")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTestTypeConfirmed(int id)
        {
            try
            {
                var success = _labManagerService.DeleteTestType(id);

                if (success)
                {
                    TempData["StatusMessage"] = "Test type successfully deleted.";
                }
                else
                {
                    TempData["StatusMessage"] = "Unable to delete test type. It may not exist.";
                }

                return RedirectToAction(nameof(TestTypes));
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"Error deleting test type: {ex.Message}";
                return RedirectToAction(nameof(TestTypes));
            }
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
            ViewBag.categories = _labManagerService.GetCategories().ToList();
            if (category == null) return NotFound();
            return View("TestCategories/Edit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTestCategory(int id, TestCategory category)
        {
            // Security check: Ensure the ID in the URL matches the hidden ID in the form
            if (id != category.Id)
            {
                return NotFound();
            }

            // ModelState.IsValid automatically checks any [Required] or [StringLength] attributes on your Model
            if (ModelState.IsValid)
            {
                try
                {
                    _labManagerService.UpdateCategory(category.Id, category.CategoryName, category.Description);

                    TempData["StatusMessage"] = "Category updated successfully.";
                    return RedirectToAction(nameof(TestCategories));
                }
                catch (Exception)
                {
                    TempData["StatusMessage"] = "Unable to update category.";
                }
            }
            return View("TestCategories/Edit", category);
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
                _labManagerService.CreateDoctor(
                    form["Name"].ToString(),
                    form["Surname"].ToString(),
                    form["HpcsaNumber"].ToString(),
                    form["Specialty"].ToString(),
                    form["EmailAddress"].ToString(),
                    form["ContactNumber"].ToString()
                );

                TempData["StatusMessage"] = "Doctor successfully registered.";
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
                var specialty = GetValue(form, "Specialty");
                var emailAddress = GetValue(form, "EmailAddress");
                var contactNumber = GetValue(form, "ContactNumber");
                _labManagerService.UpdateDoctor(id, name, surname, hpcsaNumber, specialty, emailAddress, contactNumber);
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
            var tech = _labManagerService.GetLabTechnicians().FirstOrDefault(t => t.Id == id);
            if (tech == null) return NotFound();

            // Fetch the actual names of the tests assigned to this technician
            var allTestTypes = _labManagerService.GetTestTypes();
            var assignedTestNames = allTestTypes
                .Where(t => tech.AssignedTestTypeIds.Contains(t.Id))
                .Select(t => t.TestName)
                .ToList();

            var viewModel = new NMB_HLabSys_VIEWS.ViewModels.TechnicianDetailsViewModel
            {
                Id = tech.Id,
                Name = tech.Name,
                Surname = tech.Surname,
                SouthAfricanIdNumber = tech.SouthAfricanIdNumber,
                EmployeeNumber = tech.EmployeeNumber,
                EmailAddress = tech.EmailAddress,
                ContactNumber = tech.ContactNumber,
                AssociatedTests = assignedTestNames
            };

            return View("LabTechnicians/Details",viewModel);
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
            var sampleType = _labManagerService.GetSampleType(id);
            if (sampleType == null) return NotFound();

            // Safety check: See if any test types are currently using this sample type
            bool isUsed = _labManagerService.GetTestTypes().Any(t => t.SampleTypeId == id);
            ViewBag.CanDelete = !isUsed;

            return View("SampleTypes/Delete", sampleType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSampleTypeConfirmed(int id)
        {
            try
            {
                _labManagerService.DeleteSampleType(id);
                TempData["StatusMessage"] = "Sample type deleted successfully.";
                return RedirectToAction(nameof(SampleTypes));
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"Error deleting sample type: {ex.Message}";
                return RedirectToAction(nameof(SampleTypes));
            }
        }

        [HttpGet]
        public IActionResult Orders()
        {
            // 1. Get all orders, sorted with the newest at the top
            var orders = _labManagerService.GetOrders().OrderByDescending(o => o.OrderDate).ToList();

            // 2. Data for our Smart Alerts
            var consumables = _labManagerService.GetConsumables();
            var suppliers = _labManagerService.GetSuppliers();

            // Check for low stock (Quantity <= Reorder Level)
            ViewBag.LowStockItems = consumables.Where(c => c.QuantityOnHand <= c.ReorderLevel).ToList();

            // Pass suppliers as a dictionary for easy name lookups
            ViewBag.SupplierDict = suppliers.ToDictionary(s => s.Id, s => s.SupplierName);

            return View("Orders/Index", orders);
        }

        public IActionResult OrderDetails(int id)
        {
            // Fetch the order
            var order = _labManagerService.GetOrders().FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            // Fetch the supplier name
            var supplier = _labManagerService.GetSupplier(order.SupplierId);
            ViewBag.SupplierName = supplier?.SupplierName ?? "Unknown";

            // Fetch consumable names into a fast lookup dictionary
            var consumables = _labManagerService.GetConsumables().ToDictionary(c => c.Id, c => c.ConsumableName);
            ViewBag.ConsumableDict = consumables;
            return View("Orders/Details", order);
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
                string randomSuffix = new Random().Next(1000, 9999).ToString();
                var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{randomSuffix}";

                var supplierId = int.Parse(form["SupplierId"]);

                // Create a list of tuples to hold (ConsumableId, Quantity)
                var orderedItems = new List<(int consumableId, int quantity)>();

                // Grab the checked checkboxes
                if (!string.IsNullOrWhiteSpace(form["ConsumableIds"]))
                {
                    var selectedIds = form["ConsumableIds"].ToString().Split(',');

                    foreach (var idString in selectedIds)
                    {
                        if (int.TryParse(idString, out int consumableId))
                        {
                            // Look up the specific quantity field for THIS consumable ID
                            string quantityFieldName = $"Quantity_{consumableId}";

                            if (int.TryParse(form[quantityFieldName], out int quantity) && quantity > 0)
                            {
                                orderedItems.Add((consumableId, quantity));
                            }
                        }
                    }
                }

                if (!orderedItems.Any())
                {
                    throw new Exception("You must select at least one consumable and provide a valid quantity.");
                }

                // Pass the successfully parsed items to your LabManagerService
                _labManagerService.CreateOrder(supplierId, orderedItems.ToArray(), orderNumber);

                TempData["StatusMessage"] = "Order successfully created.";
                return RedirectToAction("Orders");
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"Failed to create order: {ex.Message}";
                return RedirectToAction("CreateOrder");
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
            try
            {
                // This triggers the automation we wrote in Step 1
                _labManagerService.ReceiveOrder(id);

                TempData["StatusMessage"] = "Order marked as received. Inventory quantities have been updated automatically!";
                return RedirectToAction(nameof(Orders));
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"Failed to receive order: {ex.Message}";
                return RedirectToAction(nameof(Orders));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(int id, string cancellationReason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cancellationReason))
                {
                    throw new Exception("You must provide a reason for cancellation.");
                }

                var success = _labManagerService.CancelOrder(id, cancellationReason);

                if (success)
                {
                    TempData["StatusMessage"] = "Order was successfully cancelled.";
                }
                else
                {
                    TempData["StatusMessage"] = "Could not cancel order. It may have already been processed.";
                }

                return RedirectToAction(nameof(Orders)); // Send them back to the dashboard
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = $"Failed to cancel order: {ex.Message}";
                return RedirectToAction(nameof(OrderDetails), new { id = id });
            }
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
