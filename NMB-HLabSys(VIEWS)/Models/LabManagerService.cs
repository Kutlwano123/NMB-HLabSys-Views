using NMB_HLabSys_VIEWS_.Models;

namespace NMB_HLabSys_VIEWS_.Models
{
    public interface ILabManagerService
    {
        IEnumerable<TestCategory> GetCategories();
        TestCategory? GetCategory(int id);
        TestCategory CreateCategory(string categoryName, string description);
        TestCategory? UpdateCategory(int id, string categoryName, string description);
        bool DeleteCategory(int id);

        IEnumerable<TestType> GetTestTypes();
        TestType? GetTestType(int id);
        TestType CreateTestType(string testName, int categoryId, string sampleType, string unitOfMeasurement, string minRange, string maxRange, int turnaroundTimeMinutes, IEnumerable<int> consumableIds, IEnumerable<int> technicianIds);
        TestType? UpdateTestType(int id, string testName, int categoryId, string sampleType, string unitOfMeasurement, string minRange, string maxRange, int turnaroundTimeMinutes, IEnumerable<int> consumableIds, IEnumerable<int> technicianIds);
        bool DeleteTestType(int id);

        IEnumerable<SampleType> GetSampleTypes();
        SampleType? GetSampleType(int id);
        SampleType CreateSampleType(string name, string description);
        SampleType? UpdateSampleType(int id, string name, string description);
        bool DeleteSampleType(int id);

        IEnumerable<Consumable> GetConsumables();
        Consumable? GetConsumable(int id);
        Consumable CreateConsumable(string name, int reorderLevel, int quantityOnHand, int supplierId);
        Consumable? UpdateConsumable(int id, string name, int reorderLevel, int quantityOnHand, int supplierId);
        bool DeleteConsumable(int id);
        bool AdjustStock(int consumableId, int value, string mode);
        IEnumerable<Consumable> GetReorderAlerts();

        IEnumerable<Supplier> GetSuppliers();
        Supplier? GetSupplier(int id);
        Supplier CreateSupplier(string supplierName, string contactPerson, string emailAddress);
        Supplier? UpdateSupplier(int id, string supplierName, string contactPerson, string emailAddress);
        bool DeleteSupplier(int id);

        IEnumerable<Order> GetOrders();
        Order? GetOrder(int id);
        Order CreateOrder(int supplierId, IEnumerable<(int ConsumableId, int Quantity)> items, string? orderNumber = null);
        bool DeleteOrder(int id);
        bool ReceiveOrder(int orderId);
        bool CancelOrder(int orderId, string reason);

        IEnumerable<DoctorUser> GetDoctors();
        DoctorUser? GetDoctor(int id);
        DoctorUser CreateDoctor(string name, string surname, string hpcsaNumber, string emailAddress, string contactNumber);
        DoctorUser? UpdateDoctor(int id, string name, string surname, string hpcsaNumber, string emailAddress, string contactNumber);
        bool DeleteDoctor(int id);

        IEnumerable<LabTechnicianUser> GetLabTechnicians();
        LabTechnicianUser? GetLabTechnician(int id);
        LabTechnicianUser CreateLabTechnician(string name, string surname, string southAfricanIdNumber, string employeeNumber, string emailAddress, string contactNumber, IEnumerable<int> assignedTestTypeIds);
        LabTechnicianUser? UpdateLabTechnician(int id, string name, string surname, string southAfricanIdNumber, string employeeNumber, string emailAddress, string contactNumber, IEnumerable<int> assignedTestTypeIds);
        bool DeleteLabTechnician(int id);

        IEnumerable<ReportRow> GetReports(DateTime startDate, DateTime endDate);
        string GenerateReportPdf(DateTime startDate, DateTime endDate);
    }

    public class InMemoryLabManagerService : ILabManagerService
    {
        private readonly List<TestCategory> _categories = new();
        private readonly List<TestType> _testTypes = new();
        private readonly List<SampleType> _sampleTypes = new();
        private readonly List<Consumable> _consumables = new();
        private readonly List<Supplier> _suppliers = new();
        private readonly List<Order> _orders = new();
        private readonly List<DoctorUser> _doctors = new();
        private readonly List<LabTechnicianUser> _technicians = new();
        private readonly List<TestPerformanceRecord> _performanceRecords = new();
        private int _nextCategoryId = 1;
        private int _nextTestTypeId = 1;
        private int _nextSampleTypeId = 1;
        private int _nextConsumableId = 1;
        private int _nextSupplierId = 1;
        private int _nextOrderId = 1;
        private int _nextDoctorId = 1;
        private int _nextTechnicianId = 1;
        private int _nextOrderItemId = 1;
        private int _nextPerformanceRecordId = 1;

        public InMemoryLabManagerService()
        {
            Seed();
        }

        private void Seed()
        {
            var hematology = CreateCategory("Hematology", "Blood and cellular analysis");
            var chemistry = CreateCategory("Chemistry", "Metabolic and chemistry panels");
            var microbiology = CreateCategory("Microbiology", "Culture and sensitivity testing");

            CreateTestType("Full Blood Count", hematology.Id, "Whole Blood", "x10³/µL", "4.0", "11.0", 30, new[] { 1 }, new[] { 1, 2 });
            CreateTestType("Coagulation Studies", hematology.Id, "Plasma", "s", "10", "14", 45, new[] { 1, 2 }, new[] { 2 });
            CreateTestType("Liver Function Test", chemistry.Id, "Serum", "U/L", "5", "40", 60, new[] { 3 }, new[] { 1 });
            CreateTestType("Urine Culture", microbiology.Id, "Urine", "CFU/mL", "0", "1000", 120, new[] { 4 }, new[] { 2 });

            CreateSampleType("Whole Blood", "Whole blood sample collected in EDTA");
            CreateSampleType("Plasma", "Plasma sample suitable for coagulation testing");
            CreateSampleType("Serum", "Serum sample for chemistry testing");
            CreateSampleType("Urine", "Urine specimen collected for microbiology");

            CreateSupplier("MedSupply Co.", "Thabo Mokoena", "orders@medsupply.co.za");
            CreateSupplier("Lab Essentials", "Nandi Dube", "nandi@labessentials.co.za");
            CreateSupplier("HealthPro", "Lerato Molefe", "lerato@healthpro.co.za");

            CreateConsumable("EDTA Tubes", 20, 45, 1);
            CreateConsumable("Coagulation Reagent", 12, 18, 2);
            CreateConsumable("Serum Separator Tubes", 15, 36, 2);
            CreateConsumable("Culture Media", 10, 18, 3);

            CreateDoctor("Dr. A. Petersen", "Petersen", "HPCSA-1001", "doctor1@nmb.ac.za", "0821110001");
            CreateDoctor("Dr. S. Ngcobo", "Ngcobo", "HPCSA-1002", "doctor2@nmb.ac.za", "0821110002");

            CreateLabTechnician("Kabelo", "Mabidikama", "9001015001089", "EMP-1001", "tech1@nmb.ac.za", "0711010001", new[] { 1, 2 });
            CreateLabTechnician("Lindo", "Mthembu", "9102026002088", "EMP-1002", "tech2@nmb.ac.za", "0711010002", new[] { 3, 4 });

            CreateOrder(1, new[] { (1, 30), (4, 20) }, "ORD-1001");
            CreateOrder(2, new[] { (2, 15) }, "ORD-1002");

            _performanceRecords.Add(new TestPerformanceRecord { Id = _nextPerformanceRecordId++, CategoryId = hematology.Id, TestTypeId = 1, PerformedOn = DateTime.Today.AddDays(-2) });
            _performanceRecords.Add(new TestPerformanceRecord { Id = _nextPerformanceRecordId++, CategoryId = hematology.Id, TestTypeId = 2, PerformedOn = DateTime.Today.AddDays(-5) });
            _performanceRecords.Add(new TestPerformanceRecord { Id = _nextPerformanceRecordId++, CategoryId = chemistry.Id, TestTypeId = 3, PerformedOn = DateTime.Today.AddDays(-9) });
            _performanceRecords.Add(new TestPerformanceRecord { Id = _nextPerformanceRecordId++, CategoryId = microbiology.Id, TestTypeId = 4, PerformedOn = DateTime.Today.AddDays(-12) });
        }

        public IEnumerable<TestCategory> GetCategories() => _categories.OrderBy(x => x.CategoryName).ToList();

        public TestCategory? GetCategory(int id) => _categories.FirstOrDefault(x => x.Id == id);

        public TestCategory CreateCategory(string categoryName, string description)
        {
            if (_categories.Any(x => string.Equals(x.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A category with that name already exists.");
            }

            var category = new TestCategory { Id = _nextCategoryId++, CategoryName = categoryName, Description = description };
            _categories.Add(category);
            return category;
        }

        public TestCategory? UpdateCategory(int id, string categoryName, string description)
        {
            var category = _categories.FirstOrDefault(x => x.Id == id);
            if (category == null) return null;
            if (_categories.Any(x => x.Id != id && string.Equals(x.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A category with that name already exists.");
            }
            category.CategoryName = categoryName;
            category.Description = description;
            return category;
        }

        public bool DeleteCategory(int id)
        {
            var category = _categories.FirstOrDefault(x => x.Id == id);
            if (category == null) return false;
            if (_testTypes.Any(x => x.CategoryId == id)) return false;
            _categories.Remove(category);
            return true;
        }

        public IEnumerable<TestType> GetTestTypes() => _testTypes.OrderBy(x => x.TestName).ToList();

        public TestType? GetTestType(int id) => _testTypes.FirstOrDefault(x => x.Id == id);

        public TestType CreateTestType(string testName, int categoryId, string sampleType, string unitOfMeasurement, string minRange, string maxRange, int turnaroundTimeMinutes, IEnumerable<int> consumableIds, IEnumerable<int> technicianIds)
        {
            if (_testTypes.Any(x => string.Equals(x.TestName, testName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A test type with that name already exists.");
            }
            if (_categories.All(x => x.Id != categoryId))
            {
                throw new InvalidOperationException("The selected category does not exist.");
            }

            var testType = new TestType
            {
                Id = _nextTestTypeId++,
                TestName = testName,
                CategoryId = categoryId,
                SampleType = sampleType,
                UnitOfMeasurement = unitOfMeasurement,
                NormalRangeMin = minRange,
                NormalRangeMax = maxRange,
                TurnaroundTimeMinutes = turnaroundTimeMinutes,
                ConsumableIds = consumableIds.ToList(),
                TechnicianIds = technicianIds.ToList()
            };

            _testTypes.Add(testType);
            var category = _categories.First(x => x.Id == categoryId);
            category.TestTypeIds.Add(testType.Id);
            return testType;
        }

        public TestType? UpdateTestType(int id, string testName, int categoryId, string sampleType, string unitOfMeasurement, string minRange, string maxRange, int turnaroundTimeMinutes, IEnumerable<int> consumableIds, IEnumerable<int> technicianIds)
        {
            var testType = _testTypes.FirstOrDefault(x => x.Id == id);
            if (testType == null) return null;
            if (_testTypes.Any(x => x.Id != id && string.Equals(x.TestName, testName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A test type with that name already exists.");
            }
            if (_categories.All(x => x.Id != categoryId))
            {
                throw new InvalidOperationException("The selected category does not exist.");
            }

            testType.TestName = testName;
            testType.CategoryId = categoryId;
            testType.SampleType = sampleType;
            testType.UnitOfMeasurement = unitOfMeasurement;
            testType.NormalRangeMin = minRange;
            testType.NormalRangeMax = maxRange;
            testType.TurnaroundTimeMinutes = turnaroundTimeMinutes;
            testType.ConsumableIds = consumableIds.ToList();
            testType.TechnicianIds = technicianIds.ToList();

            var oldCategoryId = _categories.FirstOrDefault(x => x.TestTypeIds.Contains(id))?.Id;
            if (oldCategoryId != null && oldCategoryId != categoryId)
            {
                var oldCategory = _categories.First(x => x.Id == oldCategoryId);
                oldCategory.TestTypeIds.Remove(id);
            }

            var newCategory = _categories.First(x => x.Id == categoryId);
            if (!newCategory.TestTypeIds.Contains(id))
            {
                newCategory.TestTypeIds.Add(id);
            }
            return testType;
        }

        public bool DeleteTestType(int id)
        {
            var testType = _testTypes.FirstOrDefault(x => x.Id == id);
            if (testType == null) return false;
            _testTypes.Remove(testType);
            var category = _categories.FirstOrDefault(x => x.TestTypeIds.Contains(id));
            category?.TestTypeIds.Remove(id);
            return true;
        }

        public IEnumerable<SampleType> GetSampleTypes() => _sampleTypes.OrderBy(x => x.Name).ToList();

        public SampleType? GetSampleType(int id) => _sampleTypes.FirstOrDefault(x => x.Id == id);

        public SampleType CreateSampleType(string name, string description)
        {
            if (_sampleTypes.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A sample type with that name already exists.");
            }
            var sampleType = new SampleType { Id = _nextSampleTypeId++, Name = name, Description = description };
            _sampleTypes.Add(sampleType);
            return sampleType;
        }

        public SampleType? UpdateSampleType(int id, string name, string description)
        {
            var sampleType = _sampleTypes.FirstOrDefault(x => x.Id == id);
            if (sampleType == null) return null;
            if (_sampleTypes.Any(x => x.Id != id && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A sample type with that name already exists.");
            }
            sampleType.Name = name;
            sampleType.Description = description;
            return sampleType;
        }

        public bool DeleteSampleType(int id)
        {
            var sampleType = _sampleTypes.FirstOrDefault(x => x.Id == id);
            if (sampleType == null) return false;
            _sampleTypes.Remove(sampleType);
            return true;
        }

        public IEnumerable<Consumable> GetConsumables() => _consumables.OrderBy(x => x.ConsumableName).ToList();

        public Consumable? GetConsumable(int id) => _consumables.FirstOrDefault(x => x.Id == id);

        public Consumable CreateConsumable(string name, int reorderLevel, int quantityOnHand, int supplierId)
        {
            if (_consumables.Any(x => string.Equals(x.ConsumableName, name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A consumable with that name already exists.");
            }
            if (_suppliers.All(x => x.Id != supplierId))
            {
                throw new InvalidOperationException("The selected supplier does not exist.");
            }
            var consumable = new Consumable { Id = _nextConsumableId++, ConsumableName = name, ReorderLevel = reorderLevel, QuantityOnHand = quantityOnHand, SupplierId = supplierId };
            _consumables.Add(consumable);
            var supplier = _suppliers.First(x => x.Id == supplierId);
            supplier.ConsumableIds.Add(consumable.Id);
            return consumable;
        }

        public Consumable? UpdateConsumable(int id, string name, int reorderLevel, int quantityOnHand, int supplierId)
        {
            var consumable = _consumables.FirstOrDefault(x => x.Id == id);
            if (consumable == null) return null;
            if (_consumables.Any(x => x.Id != id && string.Equals(x.ConsumableName, name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A consumable with that name already exists.");
            }
            if (_suppliers.All(x => x.Id != supplierId))
            {
                throw new InvalidOperationException("The selected supplier does not exist.");
            }
            consumable.ConsumableName = name;
            consumable.ReorderLevel = reorderLevel;
            consumable.QuantityOnHand = quantityOnHand;
            consumable.SupplierId = supplierId;
            return consumable;
        }

        public bool DeleteConsumable(int id)
        {
            var consumable = _consumables.FirstOrDefault(x => x.Id == id);
            if (consumable == null) return false;
            _consumables.Remove(consumable);
            var supplier = _suppliers.FirstOrDefault(x => x.ConsumableIds.Contains(id));
            supplier?.ConsumableIds.Remove(id);
            return true;
        }

        public bool AdjustStock(int consumableId, int value, string mode)
        {
            var consumable = _consumables.FirstOrDefault(x => x.Id == consumableId);
            if (consumable == null) return false;
            if (mode.Equals("set", StringComparison.OrdinalIgnoreCase))
            {
                consumable.QuantityOnHand = value;
            }
            else if (mode.Equals("increase", StringComparison.OrdinalIgnoreCase))
            {
                consumable.QuantityOnHand += value;
            }
            else if (mode.Equals("decrease", StringComparison.OrdinalIgnoreCase))
            {
                consumable.QuantityOnHand -= value;
            }
            return true;
        }

        public IEnumerable<Consumable> GetReorderAlerts() => _consumables.Where(x => x.QuantityOnHand <= x.ReorderLevel * 1.1m).OrderBy(x => x.QuantityOnHand).ToList();

        public IEnumerable<Supplier> GetSuppliers() => _suppliers.OrderBy(x => x.SupplierName).ToList();

        public Supplier? GetSupplier(int id) => _suppliers.FirstOrDefault(x => x.Id == id);

        public Supplier CreateSupplier(string supplierName, string contactPerson, string emailAddress)
        {
            if (_suppliers.Any(x => string.Equals(x.SupplierName, supplierName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A supplier with that name already exists.");
            }
            var supplier = new Supplier { Id = _nextSupplierId++, SupplierName = supplierName, ContactPerson = contactPerson, EmailAddress = emailAddress };
            _suppliers.Add(supplier);
            return supplier;
        }

        public Supplier? UpdateSupplier(int id, string supplierName, string contactPerson, string emailAddress)
        {
            var supplier = _suppliers.FirstOrDefault(x => x.Id == id);
            if (supplier == null) return null;
            if (_suppliers.Any(x => x.Id != id && string.Equals(x.SupplierName, supplierName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A supplier with that name already exists.");
            }
            supplier.SupplierName = supplierName;
            supplier.ContactPerson = contactPerson;
            supplier.EmailAddress = emailAddress;
            return supplier;
        }

        public bool DeleteSupplier(int id)
        {
            var supplier = _suppliers.FirstOrDefault(x => x.Id == id);
            if (supplier == null) return false;
            if (_consumables.Any(x => x.SupplierId == id)) return false;
            _suppliers.Remove(supplier);
            return true;
        }

        public IEnumerable<Order> GetOrders() => _orders.OrderByDescending(x => x.OrderDate).ToList();

        public Order? GetOrder(int id) => _orders.FirstOrDefault(x => x.Id == id);

        public Order CreateOrder(int supplierId, IEnumerable<(int ConsumableId, int Quantity)> items, string? orderNumber = null)
        {
            if (_suppliers.All(x => x.Id != supplierId)) throw new InvalidOperationException("The selected supplier does not exist.");
            var order = new Order
            {
                Id = _nextOrderId++,
                SupplierId = supplierId,
                OrderNumber = string.IsNullOrWhiteSpace(orderNumber) ? $"ORD-{DateTime.Today:yyyyMMdd}-{_nextOrderId}" : orderNumber,
                OrderDate = DateTime.Today,
                Status = "Ordered"
            };
            order.Items = items.Select((item, index) => new OrderItem { Id = _nextOrderItemId++, ConsumableId = item.ConsumableId, QuantityOrdered = item.Quantity, Status = "Ordered" }).ToList();
            _orders.Add(order);
            return order;
        }

        public bool DeleteOrder(int id)
        {
            var order = _orders.FirstOrDefault(x => x.Id == id);
            if (order == null) return false;
            _orders.Remove(order);
            return true;
        }

        public bool ReceiveOrder(int orderId)
        {
            var order = _orders.FirstOrDefault(x => x.Id == orderId);
            if (order == null) return false;
            foreach (var item in order.Items)
            {
                var consumable = _consumables.FirstOrDefault(x => x.Id == item.ConsumableId);
                if (consumable != null)
                {
                    consumable.QuantityOnHand += item.QuantityOrdered;
                }
                item.Status = "Received";
                item.DateReceived = DateTime.Today;
            }
            order.Status = "Complete";
            order.DateCompleted = DateTime.Today;
            return true;
        }

        public bool CancelOrder(int orderId, string reason)
        {
            var order = _orders.FirstOrDefault(x => x.Id == orderId);
            if (order == null) return false;
            order.Status = "Cancelled";
            order.CancellationReason = reason;
            order.DateCancelled = DateTime.Today;
            foreach (var item in order.Items)
            {
                item.Status = "Cancelled";
                item.CancellationReason = reason;
                item.DateCancelled = DateTime.Today;
            }
            return true;
        }

        public IEnumerable<DoctorUser> GetDoctors() => _doctors.OrderBy(x => x.Surname).ToList();

        public DoctorUser? GetDoctor(int id) => _doctors.FirstOrDefault(x => x.Id == id);

        public DoctorUser CreateDoctor(string name, string surname, string hpcsaNumber, string emailAddress, string contactNumber)
        {
            if (_doctors.Any(x => string.Equals(x.HpcsaNumber, hpcsaNumber, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A doctor with that HPCSA number already exists.");
            }
            var doctor = new DoctorUser
            {
                Id = _nextDoctorId++,
                Name = name,
                Surname = surname,
                HpcsaNumber = hpcsaNumber,
                EmailAddress = emailAddress,
                ContactNumber = contactNumber,
                Password = GeneratePassword(),
                MustChangePassword = true
            };
            _doctors.Add(doctor);
            return doctor;
        }

        public DoctorUser? UpdateDoctor(int id, string name, string surname, string hpcsaNumber, string emailAddress, string contactNumber)
        {
            var doctor = _doctors.FirstOrDefault(x => x.Id == id);
            if (doctor == null) return null;
            if (_doctors.Any(x => x.Id != id && string.Equals(x.HpcsaNumber, hpcsaNumber, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A doctor with that HPCSA number already exists.");
            }
            doctor.Name = name;
            doctor.Surname = surname;
            doctor.HpcsaNumber = hpcsaNumber;
            doctor.EmailAddress = emailAddress;
            doctor.ContactNumber = contactNumber;
            return doctor;
        }

        public bool DeleteDoctor(int id)
        {
            var doctor = _doctors.FirstOrDefault(x => x.Id == id);
            if (doctor == null) return false;
            _doctors.Remove(doctor);
            return true;
        }

        public IEnumerable<LabTechnicianUser> GetLabTechnicians() => _technicians.OrderBy(x => x.Surname).ToList();

        public LabTechnicianUser? GetLabTechnician(int id) => _technicians.FirstOrDefault(x => x.Id == id);

        public LabTechnicianUser CreateLabTechnician(string name, string surname, string southAfricanIdNumber, string employeeNumber, string emailAddress, string contactNumber, IEnumerable<int> assignedTestTypeIds)
        {
            if (_technicians.Any(x => string.Equals(x.SouthAfricanIdNumber, southAfricanIdNumber, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A technician with that ID number already exists.");
            }
            if (!assignedTestTypeIds.Any()) throw new InvalidOperationException("At least one test type must be assigned.");
            var technician = new LabTechnicianUser
            {
                Id = _nextTechnicianId++,
                Name = name,
                Surname = surname,
                SouthAfricanIdNumber = southAfricanIdNumber,
                EmployeeNumber = employeeNumber,
                EmailAddress = emailAddress,
                ContactNumber = contactNumber,
                Password = GeneratePassword(),
                MustChangePassword = true,
                AssignedTestTypeIds = assignedTestTypeIds.ToList()
            };
            _technicians.Add(technician);
            return technician;
        }

        public LabTechnicianUser? UpdateLabTechnician(int id, string name, string surname, string southAfricanIdNumber, string employeeNumber, string emailAddress, string contactNumber, IEnumerable<int> assignedTestTypeIds)
        {
            var technician = _technicians.FirstOrDefault(x => x.Id == id);
            if (technician == null) return null;
            if (_technicians.Any(x => x.Id != id && string.Equals(x.SouthAfricanIdNumber, southAfricanIdNumber, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("A technician with that ID number already exists.");
            }
            if (!assignedTestTypeIds.Any()) throw new InvalidOperationException("At least one test type must be assigned.");
            technician.Name = name;
            technician.Surname = surname;
            technician.SouthAfricanIdNumber = southAfricanIdNumber;
            technician.EmployeeNumber = employeeNumber;
            technician.EmailAddress = emailAddress;
            technician.ContactNumber = contactNumber;
            technician.AssignedTestTypeIds = assignedTestTypeIds.ToList();
            return technician;
        }

        public bool DeleteLabTechnician(int id)
        {
            var technician = _technicians.FirstOrDefault(x => x.Id == id);
            if (technician == null) return false;
            _technicians.Remove(technician);
            return true;
        }

        public IEnumerable<ReportRow> GetReports(DateTime startDate, DateTime endDate)
        {
            return _performanceRecords
                .Where(x => x.PerformedOn >= startDate && x.PerformedOn <= endDate)
                .GroupBy(x => x.CategoryId)
                .Select(g => new ReportRow
                {
                    CategoryName = _categories.FirstOrDefault(c => c.Id == g.Key)?.CategoryName ?? "Unknown",
                    TotalTestsPerformed = g.Count()
                })
                .OrderByDescending(x => x.TotalTestsPerformed)
                .ToList();
        }

        public string GenerateReportPdf(DateTime startDate, DateTime endDate)
        {
            var rows = GetReports(startDate, endDate).ToList();
            var lines = new List<string>
            {
                "%PDF-1.4",
                "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj",
                "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj",
                "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj",
                "4 0 obj << /Length 0 >> stream\nendstream endobj",
                "5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj"
            };
            var content = string.Join(Environment.NewLine, lines);
            return content;
        }

        private static string GeneratePassword() => "Temp" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
    }
}
