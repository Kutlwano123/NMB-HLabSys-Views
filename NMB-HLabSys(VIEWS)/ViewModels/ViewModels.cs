// ============================================================
//  ViewModels.cs  —  NMB_HLabSys (VIEWS)
//  All view models for the LabTechnician subsystem.
//  No EF attributes. No model references.
// ============================================================
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NMB_HLabSys_VIEWS.ViewModels
{
    public class LabDashboardViewModel
    {
        public int SelectedByTechnician { get; set; }
        public int WaitingToBeSelected { get; set; }
        public int WaitingToBeVerified { get; set; }
        public int WaitingToBeReviewed { get; set; }
        public int UrgentTests { get; set; }
        public int OverdueTests { get; set; }
        public int NearingLimit { get; set; }

        // Notifications
        public List<NotificationViewModel> RecentNotifications { get; set; } = new();
        public int UnreadNotificationCount { get; set; }

        // Consumables
        public List<ConsumableInventoryViewModel> CriticalConsumables { get; set; } = new();
    }

    public class ResultCaptureViewModel
    {
        public int TestRequestId { get; set; }
        public string RequestNumber { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string TestName { get; set; } = "";
        public string UnitOfMeasurement { get; set; } = "";
        public double MinNormalRange { get; set; }
        public double MaxNormalRange { get; set; }
        public string ClinicalNotes { get; set; } = "";
        public bool IsOverdue { get; set; }

        public List<string> MedicalConditions { get; set; } = new();
        public List<string> Allergies { get; set; } = new();
        public List<string> CurrentMedications { get; set; } = new();

        // Required consumables for this test
        public List<ConsumableRequirementViewModel> RequiredConsumables { get; set; } = new();

        // Previous data for recapture
        public string? PreviousResultData { get; set; }
        public string? PreviousTechnicianNotes { get; set; }
        public bool IsRecapture { get; set; }

        [Required(ErrorMessage = "Result value is required.")]
        public string? ResultData { get; set; }
        public string? TechnicianNotes { get; set; }
    }

    public class ResultVerificationViewModel
    {
        public int TestResultId { get; set; }
        public string? RequestNumber { get; set; }
        public string? PatientName { get; set; }
        public string? TestName { get; set; }
        public string? CapturedByTechnician { get; set; }
        public string? ResultData { get; set; }
        public double MinNormalRange { get; set; }
        public double MaxNormalRange { get; set; }
        public string? UnitOfMeasurement { get; set; }
        public bool IsOutOfRange { get; set; }
    }

    // ── Notification ViewModels ────────────────────────────

    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string NotificationType { get; set; } = "";  // "Urgent", "TAT", "Overdue", "Rejection"
        public System.DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public int? RelatedTestRequestId { get; set; }
        public int? RelatedTestResultId { get; set; }

        public string TypeBadgeClass
        {
            get => NotificationType switch
            {
                "Urgent" => "badge bg-danger",
                "TAT" => "badge bg-warning text-dark",
                "Overdue" => "badge bg-danger",
                "Rejection" => "badge bg-warning text-dark",
                _ => "badge bg-secondary"
            };
        }
    }

    public class NotificationCenterViewModel
    {
        public List<NotificationViewModel> Notifications { get; set; } = new();
        public int UnreadCount { get; set; }
    }

    // ── Consumable ViewModels ──────────────────────────────

    public class ConsumableInventoryViewModel
    {
        public int ConsumableId { get; set; }
        public string ConsumableName { get; set; } = "";
        public string Unit { get; set; } = "";
        public int QuantityInStock { get; set; }
        public int MinimumThreshold { get; set; }
        public bool IsLow => QuantityInStock <= MinimumThreshold;
    }

    public class ConsumableRequirementViewModel
    {
        public int RequirementId { get; set; }
        public int TestTypeId { get; set; }
        public string TestName { get; set; } = "";
        public int ConsumableId { get; set; }
        public string ConsumableName { get; set; } = "";
        public int QuantityRequired { get; set; }
        public string Unit { get; set; } = "";
        public int AvailableQuantity { get; set; }
        public bool IsSufficientStock => AvailableQuantity >= QuantityRequired;
    }

    public class ConsumableTrackingViewModel
    {
        public int TestTypeId { get; set; }
        public string TestName { get; set; } = "";
        public List<ConsumableRequirementViewModel> Requirements { get; set; } = new();
        public bool AllConsumablesAvailable { get; set; }
    }

    // ── Filtering & Recapture ViewModels ───────────────────

    public class RejectionHistoryViewModel
    {
        public int RejectionLogId { get; set; }
        public int TestResultId { get; set; }
        public int TestRequestId { get; set; }
        public string RequestNumber { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string TestName { get; set; } = "";
        public string RejectedBy { get; set; } = "";
        public string RejectionNotes { get; set; } = "";
        public string PreviousResultData { get; set; } = "";
        public string PreviousTechnicianNotes { get; set; } = "";
        public System.DateTime RejectedAt { get; set; }
        public bool CanRecapture { get; set; } = true;
    }

    public class RecaptureFormViewModel
    {
        public int TestResultId { get; set; }
        public int TestRequestId { get; set; }
        public string RequestNumber { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string TestName { get; set; } = "";
        public string UnitOfMeasurement { get; set; } = "";
        public double MinNormalRange { get; set; }
        public double MaxNormalRange { get; set; }
        public string ClinicalNotes { get; set; } = "";

        // Previous rejection context
        public string RejectedBy { get; set; } = "";
        public string RejectionNotes { get; set; } = "";
        public string PreviousResultData { get; set; } = "";
        public string PreviousTechnicianNotes { get; set; } = "";

        // New capture data
        [Required(ErrorMessage = "New result value is required.")]
        public string? NewResultData { get; set; }
        public string? NewTechnicianNotes { get; set; }

        public List<string> MedicalConditions { get; set; } = new();
        public List<string> Allergies { get; set; } = new();
        public List<string> CurrentMedications { get; set; } = new();
    }

    public class TableFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public string? UrgencyFilter { get; set; }  // "STAT", "Urgent", "Routine"
        public string? StatusFilter { get; set; }
        public string? CategoryFilter { get; set; }
        public System.DateTime? FromDate { get; set; }
        public System.DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } = "CreatedDate";
        public string? SortOrder { get; set; } = "desc";

        public List<string> AvailableUrgencies { get; } = new() { "STAT", "Urgent", "Routine" };
        public List<string> AvailableStatuses { get; set; } = new();
        public List<string> AvailableCategories { get; set; } = new();
    }

    public class FilteredTestListViewModel
    {
        public List<TestListItemViewModel> Tests { get; set; } = new();
        public TableFilterViewModel CurrentFilter { get; set; } = new();
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
    }

    public class TestListItemViewModel
    {
        public int TestRequestId { get; set; }
        public string RequestNumber { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string TestName { get; set; } = "";
        public string Urgency { get; set; } = "";
        public string Status { get; set; } = "";
        public string Category { get; set; } = "";
        public System.DateTime? DueDate { get; set; }
        public System.DateTime RequestDate { get; set; }
        public bool IsOverdue { get; set; }
    }
}
