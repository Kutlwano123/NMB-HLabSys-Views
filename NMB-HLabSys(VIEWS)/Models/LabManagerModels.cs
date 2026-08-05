using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys_VIEWS_.Models
{
    public class TestCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> TestTypeIds { get; set; } = new();
    }

    public class TestType
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string SampleType { get; set; } = string.Empty;
        public string UnitOfMeasurement { get; set; } = string.Empty;
        public string NormalRangeMin { get; set; } = string.Empty;
        public string NormalRangeMax { get; set; } = string.Empty;
        public int TurnaroundTimeMinutes { get; set; }
        public List<int> ConsumableIds { get; set; } = new();
        public List<int> TechnicianIds { get; set; } = new();
    }

    public class SampleType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class Consumable
    {
        public int Id { get; set; }
        public string ConsumableName { get; set; } = string.Empty;
        public int ReorderLevel { get; set; }
        public int QuantityOnHand { get; set; }
        public int SupplierId { get; set; }
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public List<int> ConsumableIds { get; set; } = new();
    }

    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Today;
        public string Status { get; set; } = "Ordered";
        public string? CancellationReason { get; set; }
        public DateTime? DateCancelled { get; set; }
        public DateTime? DateCompleted { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ConsumableId { get; set; }
        public int QuantityOrdered { get; set; }
        public string Status { get; set; } = "Ordered";
        public string? CancellationReason { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? DateCancelled { get; set; }
    }

    public class DoctorUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string HpcsaNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool MustChangePassword { get; set; } = true;
    }

    public class LabTechnicianUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string SouthAfricanIdNumber { get; set; } = string.Empty;
        public string EmployeeNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool MustChangePassword { get; set; } = true;
        public List<int> AssignedTestTypeIds { get; set; } = new();
    }

    public class TestPerformanceRecord
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int TestTypeId { get; set; }
        public DateTime PerformedOn { get; set; }
    }

    public class ReportRow
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalTestsPerformed { get; set; }
    }

    public class StockAdjustmentViewModel
    {
        [Required]
        public int ConsumableId { get; set; }
        [Required]
        public int AdjustmentValue { get; set; }
        public string AdjustmentMode { get; set; } = "increase";
    }

    public class ReportsViewModel
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public List<ReportRow> ReportRows { get; set; } = new();
    }
}
