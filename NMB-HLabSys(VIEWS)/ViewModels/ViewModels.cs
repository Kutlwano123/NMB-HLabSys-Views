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
}
