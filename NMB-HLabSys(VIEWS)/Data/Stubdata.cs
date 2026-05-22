// ============================================================
//  StubData.cs  —  NMB_HLabSys (VIEWS)
//  All in-memory dummy data. No EF Core. No database.
// ============================================================
using System;
using System.Collections.Generic;

namespace NMB_HLabSys_VIEWS.Data
{
    // ── Lightweight stub models (no EF attributes needed) ────

    public class StubTestType
    {
        public int TestTypeId { get; set; }
        public string TestName { get; set; } = "";
        public string UnitOfMeasurement { get; set; } = "";
        public string MinNormalRange { get; set; } = "0";
        public string MaxNormalRange { get; set; } = "0";
        public StubTestCategory? TestCategory { get; set; }
    }

    public class StubTestCategory
    {
        public int TestCategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }

    public class StubRequestStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = "";
    }

    public class StubResultStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = "";
    }

    public class StubProgressStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = "";
    }

    public class StubTestRequest
    {
        public int TestRequestId { get; set; }
        public string RequestNumber { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string Urgency { get; set; } = "Routine";
        public DateTime RequestDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string ClinicalNotes { get; set; } = "";
        public string AssignedTechnicianUserId { get; set; } = "";
        public string RequestingDoctorEmail { get; set; } = "";
        public string PatientConditionsSnapshot { get; set; } = "";
        public string PatientAllergiesSnapshot { get; set; } = "";
        public string PatientMedicationsSnapshot { get; set; } = "";

        public int TestTypeId { get; set; }
        public int? RequestStatusLookupId { get; set; }
        public int? TestProgressStatusLookupId { get; set; }

        // Navigation stubs
        public StubTestType? TestType { get; set; }
        public StubRequestStatus? RequestStatus { get; set; }
        public StubProgressStatus? TestProgressStatus { get; set; }

        public List<StubTestResult> TestResults { get; set; } = new();

        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now;
    }

    public class StubTestResult
    {
        public int TestResultId { get; set; }
        public int TestRequestId { get; set; }
        public int TestTypeId { get; set; }
        public string ResultData { get; set; } = "";
        public string Notes { get; set; } = "";
        public string CapturedBy { get; set; } = "";
        public DateTime? CapturedAt { get; set; }
        public string VerifiedBy { get; set; } = "";
        public DateTime? VerifiedAt { get; set; }
        public int? ResultStatusLookupId { get; set; }

        // Navigation stubs
        public StubTestRequest? TestRequest { get; set; }
        public StubResultStatus? ResultStatus { get; set; }
    }

    public class StubRejectionLog
    {
        public int RejectionLogId { get; set; }
        public int TestResultId { get; set; }
        public string RejectedBy { get; set; } = "";
        public string RejectionNotes { get; set; } = "";
        public string PreviousResultData { get; set; } = "";
        public string PreviousTechnicianNotes { get; set; } = "";
        public DateTime RejectedAt { get; set; }
    }

    public class StubTechnicianAssignment
    {
        public int AssignmentId { get; set; }
        public string TechnicianUserId { get; set; } = "";
        public int TestTypeId { get; set; }
    }

    // ── Status name constants ─────────────────────────────────

    public static class StatusNames
    {
        public const string Pending = "Pending";
        public const string SamplesReceived = "Samples Received";
        public const string Selected = "Selected";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";

        public const string ProgressSubmitted = "Submitted";
        public const string ProgressInProgress = "In Progress";
        public const string ProgressCompleted = "Completed";
        public const string ProgressVerified = "Verified";
        public const string ProgressToBeReviewed = "To Be Reviewed";

        public const string ResultCaptured = "Captured";
        public const string ResultVerified = "Verified";
        public const string ResultToBeReviewed = "To Be Reviewed";
    }

    // ── The single in-memory store ────────────────────────────

    public static class StubData
    {
        // ── Lookup tables ──────────────────────────────────────

        public static List<StubRequestStatus> RequestStatuses { get; } = new()
        {
            new() { Id = 1, StatusName = StatusNames.Pending },
            new() { Id = 2, StatusName = StatusNames.SamplesReceived },
            new() { Id = 3, StatusName = StatusNames.Selected },
            new() { Id = 4, StatusName = StatusNames.InProgress },
            new() { Id = 5, StatusName = StatusNames.Completed },
        };

        public static List<StubResultStatus> ResultStatuses { get; } = new()
        {
            new() { Id = 1, StatusName = StatusNames.ResultCaptured },
            new() { Id = 2, StatusName = StatusNames.ResultVerified },
            new() { Id = 3, StatusName = StatusNames.ResultToBeReviewed },
        };

        public static List<StubProgressStatus> ProgressStatuses { get; } = new()
        {
            new() { Id = 1, StatusName = StatusNames.ProgressSubmitted },
            new() { Id = 2, StatusName = StatusNames.ProgressInProgress },
            new() { Id = 3, StatusName = StatusNames.ProgressCompleted },
            new() { Id = 4, StatusName = StatusNames.ProgressVerified },
            new() { Id = 5, StatusName = StatusNames.ProgressToBeReviewed },
        };

        // ── Test types ─────────────────────────────────────────

        public static List<StubTestType> TestTypes { get; } = new()
        {
            new() { TestTypeId = 1, TestName = "Full Blood Count",   UnitOfMeasurement = "×10⁹/L", MinNormalRange = "4.5",  MaxNormalRange = "11.0", TestCategory = new() { TestCategoryId = 1, CategoryName = "Haematology" } },
            new() { TestTypeId = 2, TestName = "Haemoglobin",        UnitOfMeasurement = "g/dL",   MinNormalRange = "12.0", MaxNormalRange = "17.5", TestCategory = new() { TestCategoryId = 1, CategoryName = "Haematology" } },
            new() { TestTypeId = 3, TestName = "Platelet Count",     UnitOfMeasurement = "×10⁹/L", MinNormalRange = "150",  MaxNormalRange = "400",  TestCategory = new() { TestCategoryId = 1, CategoryName = "Haematology" } },
            new() { TestTypeId = 4, TestName = "Prothrombin Time",   UnitOfMeasurement = "seconds", MinNormalRange = "11",   MaxNormalRange = "14",   TestCategory = new() { TestCategoryId = 2, CategoryName = "Coagulation" } },
            new() { TestTypeId = 5, TestName = "ESR",                UnitOfMeasurement = "mm/hr",  MinNormalRange = "0",    MaxNormalRange = "20",   TestCategory = new() { TestCategoryId = 1, CategoryName = "Haematology" } },
        };

        // ── Technician assignments ─────────────────────────────
        // "demo-tech" is the stub UserId used when no auth is present

        public static List<StubTechnicianAssignment> TechnicianAssignments { get; } = new()
        {
            new() { AssignmentId = 1, TechnicianUserId = "demo-tech", TestTypeId = 1 },
            new() { AssignmentId = 2, TechnicianUserId = "demo-tech", TestTypeId = 2 },
            new() { AssignmentId = 3, TechnicianUserId = "demo-tech", TestTypeId = 3 },
            new() { AssignmentId = 4, TechnicianUserId = "demo-tech", TestTypeId = 4 },
            new() { AssignmentId = 5, TechnicianUserId = "demo-tech", TestTypeId = 5 },
        };

        // ── Test requests ──────────────────────────────────────

        public static List<StubTestRequest> TestRequests { get; } = new()
        {
            new()
            {
                TestRequestId  = 1,
                RequestNumber  = "REQ-2026-001",
                PatientName    = "Thabo Nkosi",
                Urgency        = "STAT",
                RequestDate    = DateTime.Now.AddDays(-3),
                DueDate        = DateTime.Now.AddHours(-6),   // overdue
                ClinicalNotes  = "Patient presents with fatigue and pallor.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.smith@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Anaemia; Iron Deficiency",
                PatientAllergiesSnapshot   = "Penicillin",
                PatientMedicationsSnapshot = "Ferrous Sulphate; Folic Acid",
                TestTypeId             = 2,
                RequestStatusLookupId  = 3,   // Selected
                TestProgressStatusLookupId = 2,
            },
            new()
            {
                TestRequestId  = 2,
                RequestNumber  = "REQ-2026-002",
                PatientName    = "Lindiwe Dlamini",
                Urgency        = "Urgent",
                RequestDate    = DateTime.Now.AddDays(-1),
                DueDate        = DateTime.Now.AddHours(4),
                ClinicalNotes  = "Routine pre-op screening.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.jones@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Hypertension",
                PatientAllergiesSnapshot   = "",
                PatientMedicationsSnapshot = "Amlodipine",
                TestTypeId             = 1,
                RequestStatusLookupId  = 1,   // Pending
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 3,
                RequestNumber  = "REQ-2026-003",
                PatientName    = "Sipho Mokoena",
                Urgency        = "Routine",
                RequestDate    = DateTime.Now.AddDays(-2),
                DueDate        = DateTime.Now.AddDays(1),
                ClinicalNotes  = "Follow-up on thrombocytopenia.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.abrahams@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Thrombocytopenia",
                PatientAllergiesSnapshot   = "Aspirin; NSAIDs",
                PatientMedicationsSnapshot = "Prednisolone",
                TestTypeId             = 3,
                RequestStatusLookupId  = 2,   // Samples Received
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 4,
                RequestNumber  = "REQ-2026-004",
                PatientName    = "Ayanda Petersen",
                Urgency        = "Routine",
                RequestDate    = DateTime.Now.AddDays(-5),
                DueDate        = DateTime.Now.AddDays(-1),  // overdue
                ClinicalNotes  = "Monitoring warfarin therapy.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.van_der_berg@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Atrial Fibrillation",
                PatientAllergiesSnapshot   = "",
                PatientMedicationsSnapshot = "Warfarin",
                TestTypeId             = 4,
                RequestStatusLookupId  = 3,   // Selected
                TestProgressStatusLookupId = 2,
            },
            new()
            {
                TestRequestId  = 5,
                RequestNumber  = "REQ-2026-005",
                PatientName    = "Nokwanda Zulu",
                Urgency        = "Routine",
                RequestDate    = DateTime.Now.AddDays(-1),
                DueDate        = DateTime.Now.AddDays(3),
                ClinicalNotes  = "General wellness check.",
                AssignedTechnicianUserId = "",   // not yet assigned
                RequestingDoctorEmail    = "dr.smith@nmbhdl.co.za",
                PatientConditionsSnapshot  = "",
                PatientAllergiesSnapshot   = "",
                PatientMedicationsSnapshot = "",
                TestTypeId             = 5,
                RequestStatusLookupId  = 1,   // Pending
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 6,
                RequestNumber  = "REQ-2026-006",
                PatientName    = "Mandla Khumalo",
                Urgency        = "STAT",
                RequestDate    = DateTime.Now.AddHours(-12),
                DueDate        = DateTime.Now.AddHours(2),
                ClinicalNotes  = "Emergency patient assessment - chest pain.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.patel@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Chest Pain; Cardiac Risk",
                PatientAllergiesSnapshot   = "Latex",
                PatientMedicationsSnapshot = "Aspirin; Nitroglycerin",
                TestTypeId             = 1,
                RequestStatusLookupId  = 1,   // Pending
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 7,
                RequestNumber  = "REQ-2026-007",
                PatientName    = "Naledi Mohlomi",
                Urgency        = "Urgent",
                RequestDate    = DateTime.Now.AddDays(-4),
                DueDate        = DateTime.Now.AddHours(-8),  // overdue
                ClinicalNotes  = "Monitoring glucose levels - diabetic patient.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.mkhize@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Diabetes Type 2; Hypertension",
                PatientAllergiesSnapshot   = "",
                PatientMedicationsSnapshot = "Metformin; Lisinopril",
                TestTypeId             = 5,
                RequestStatusLookupId  = 2,   // Samples Received
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 8,
                RequestNumber  = "REQ-2026-008",
                PatientName    = "Bongiwe Ndlela",
                Urgency        = "Routine",
                RequestDate    = DateTime.Now.AddDays(-6),
                DueDate        = DateTime.Now.AddDays(-2),  // overdue
                ClinicalNotes  = "Routine coagulation profile.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.sithole@nmbhdl.co.za",
                PatientConditionsSnapshot  = "",
                PatientAllergiesSnapshot   = "Sulpha drugs",
                PatientMedicationsSnapshot = "Heparin",
                TestTypeId             = 4,
                RequestStatusLookupId  = 3,   // Selected
                TestProgressStatusLookupId = 2,
            },
            new()
            {
                TestRequestId  = 9,
                RequestNumber  = "REQ-2026-009",
                PatientName    = "Tshepo Makhanya",
                Urgency        = "Urgent",
                RequestDate    = DateTime.Now.AddHours(-6),
                DueDate        = DateTime.Now.AddHours(18),
                ClinicalNotes  = "Post-operative follow-up - check for bleeding.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.thompson@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Post-operative; Recent Surgery",
                PatientAllergiesSnapshot   = "",
                PatientMedicationsSnapshot = "Antibiotics; Pain relief",
                TestTypeId             = 3,
                RequestStatusLookupId  = 1,   // Pending
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 10,
                RequestNumber  = "REQ-2026-010",
                PatientName    = "Zanele Ntuli",
                Urgency        = "Routine",
                RequestDate    = DateTime.Now.AddDays(-3),
                DueDate        = DateTime.Now.AddDays(2),
                ClinicalNotes  = "Annual health screening - normal patient.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.naidoo@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Hypertension - Controlled",
                PatientAllergiesSnapshot   = "Penicillin",
                PatientMedicationsSnapshot = "Amlodipine",
                TestTypeId             = 2,
                RequestStatusLookupId  = 2,   // Samples Received
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 11,
                RequestNumber  = "REQ-2026-011",
                PatientName    = "Kamal Hassan",
                Urgency        = "STAT",
                RequestDate    = DateTime.Now.AddHours(-3),
                DueDate        = DateTime.Now.AddHours(1),  // about to be overdue
                ClinicalNotes  = "Acute kidney injury assessment.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.fischer@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Acute Kidney Injury",
                PatientAllergiesSnapshot   = "Contrast media",
                PatientMedicationsSnapshot = "IV Fluids; Dialysis scheduled",
                TestTypeId             = 1,
                RequestStatusLookupId  = 1,   // Pending
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 12,
                RequestNumber  = "REQ-2026-012",
                PatientName    = "Amara Okonkwo",
                Urgency        = "Routine",
                RequestDate    = DateTime.Now.AddDays(-7),
                DueDate        = DateTime.Now.AddDays(-1),  // overdue
                ClinicalNotes  = "Drug interaction check - multiple medications.",
                AssignedTechnicianUserId = "",   // unassigned
                RequestingDoctorEmail    = "dr.brown@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Polypharmacy",
                PatientAllergiesSnapshot   = "Multiple drug allergies",
                PatientMedicationsSnapshot = "Complex medication regimen",
                TestTypeId             = 5,
                RequestStatusLookupId  = 1,   // Pending
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 13,
                RequestNumber  = "REQ-2026-013",
                PatientName    = "Ibrahim Khalil",
                Urgency        = "Urgent",
                RequestDate    = DateTime.Now.AddDays(-2),
                DueDate        = DateTime.Now.AddHours(12),
                ClinicalNotes  = "Bleeding disorder investigation.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.williams@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Bleeding Tendency",
                PatientAllergiesSnapshot   = "",
                PatientMedicationsSnapshot = "None reported",
                TestTypeId             = 4,
                RequestStatusLookupId  = 3,   // Selected
                TestProgressStatusLookupId = 2,
            },
            new()
            {
                TestRequestId  = 14,
                RequestNumber  = "REQ-2026-014",
                PatientName    = "Sophia Okafor",
                Urgency        = "Routine",
                RequestDate    = DateTime.Now.AddDays(-1),
                DueDate        = DateTime.Now.AddDays(5),
                ClinicalNotes  = "Routine blood donation screening.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.grant@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Blood donor screening",
                PatientAllergiesSnapshot   = "",
                PatientMedicationsSnapshot = "",
                TestTypeId             = 2,
                RequestStatusLookupId  = 2,   // Samples Received
                TestProgressStatusLookupId = 1,
            },
            new()
            {
                TestRequestId  = 15,
                RequestNumber  = "REQ-2026-015",
                PatientName    = "Marcus De Wit",
                Urgency        = "STAT",
                RequestDate    = DateTime.Now.AddHours(-2),
                DueDate        = DateTime.Now.AddHours(6),
                ClinicalNotes  = "Critical patient - sepsis screening.",
                AssignedTechnicianUserId = "demo-tech",
                RequestingDoctorEmail    = "dr.kumar@nmbhdl.co.za",
                PatientConditionsSnapshot  = "Sepsis; ICU Patient",
                PatientAllergiesSnapshot   = "Cephalosporin",
                PatientMedicationsSnapshot = "Broad-spectrum antibiotics",
                TestTypeId             = 3,
                RequestStatusLookupId  = 1,   // Pending
                TestProgressStatusLookupId = 1,
            },
        };

        // ── Test results ───────────────────────────────────────

        public static List<StubTestResult> TestResults { get; } = new()
        {
            new()
            {
                TestResultId    = 1,
                TestRequestId   = 1,
                TestTypeId      = 2,
                ResultData      = "7.2",    // below normal — out of range
                Notes           = "Patient appeared pale; venipuncture difficult.",
                CapturedBy      = "T. Moabi",
                CapturedAt      = DateTime.Now.AddHours(-2),
                ResultStatusLookupId = 1,   // Captured — awaiting verification
            },
            new()
            {
                TestResultId    = 2,
                TestRequestId   = 4,
                TestTypeId      = 4,
                ResultData      = "13.1",   // within range
                Notes           = "Within therapeutic range.",
                CapturedBy      = "K. Sithole",
                CapturedAt      = DateTime.Now.AddDays(-1),
                VerifiedBy      = "S. Dlamini",
                VerifiedAt      = DateTime.Now.AddHours(-12),
                ResultStatusLookupId = 2,   // Verified
            },
            new()
            {
                TestResultId    = 3,
                TestRequestId   = 6,
                TestTypeId      = 1,
                ResultData      = "6.8",    // slightly low
                Notes           = "Patient extremely anxious - mild haemolysis noted in sample.",
                CapturedBy      = "T. Moabi",
                CapturedAt      = DateTime.Now.AddHours(-4),
                ResultStatusLookupId = 3,   // To Be Reviewed - needs recapture due to haemolysis
            },
            new()
            {
                TestResultId    = 4,
                TestRequestId   = 7,
                TestTypeId      = 5,
                ResultData      = "18.5",   // elevated
                Notes           = "Patient fasting state confirmed. Result consistent with clinical picture.",
                CapturedBy      = "K. Sithole",
                CapturedAt      = DateTime.Now.AddDays(-2),
                VerifiedBy      = "S. Dlamini",
                VerifiedAt      = DateTime.Now.AddDays(-1),
                ResultStatusLookupId = 2,   // Verified
            },
            new()
            {
                TestResultId    = 5,
                TestRequestId   = 8,
                TestTypeId      = 4,
                ResultData      = "15.2",   // slightly elevated
                Notes           = "Patient on warfarin - monitoring therapeutic range.",
                CapturedBy      = "M. Sharma",
                CapturedAt      = DateTime.Now.AddDays(-3),
                ResultStatusLookupId = 3,   // To Be Reviewed
            },
            new()
            {
                TestResultId    = 6,
                TestRequestId   = 10,
                TestTypeId      = 2,
                ResultData      = "13.9",   // within range
                Notes           = "Normal result - no clinical concerns.",
                CapturedBy      = "K. Sithole",
                CapturedAt      = DateTime.Now.AddHours(-8),
                ResultStatusLookupId = 1,   // Captured — awaiting verification
            },
            new()
            {
                TestResultId    = 7,
                TestRequestId   = 13,
                TestTypeId      = 4,
                ResultData      = "18.5",   // abnormal
                Notes           = "Prolonged PT - suspect factor deficiency. Recommend specialized testing.",
                CapturedBy      = "T. Moabi",
                CapturedAt      = DateTime.Now.AddHours(-6),
                ResultStatusLookupId = 1,   // Captured — awaiting verification
            },
            new()
            {
                TestResultId    = 8,
                TestRequestId   = 14,
                TestTypeId      = 2,
                ResultData      = "12.5",   // low end of normal
                Notes           = "Suitable for blood donation - meets criteria.",
                CapturedBy      = "M. Sharma",
                CapturedAt      = DateTime.Now.AddHours(-10),
                ResultStatusLookupId = 1,   // Captured — awaiting verification
            },
            new()
            {
                TestResultId    = 9,
                TestRequestId   = 9,
                TestTypeId      = 3,
                ResultData      = "175",   // elevated
                Notes           = "Sample integrity questionable - possible contamination. Recapture recommended.",
                CapturedBy      = "K. Sithole",
                CapturedAt      = DateTime.Now.AddHours(-5),
                ResultStatusLookupId = 3,   // To Be Reviewed - requires recapture
            },
            new()
            {
                TestResultId    = 10,
                TestRequestId   = 11,
                TestTypeId      = 1,
                ResultData      = "5.2",   // within range
                Notes           = "Baseline assessment for acute kidney injury.",
                CapturedBy      = "T. Moabi",
                CapturedAt      = DateTime.Now.AddHours(-1),
                ResultStatusLookupId = 1,   // Captured — awaiting verification
            },
            new()
            {
                TestResultId    = 11,
                TestRequestId   = 15,
                TestTypeId      = 3,
                ResultData      = "225",   // elevated
                Notes           = "Critically elevated - consistent with sepsis response.",
                CapturedBy      = "M. Sharma",
                CapturedAt      = DateTime.Now.AddHours(-3),
                ResultStatusLookupId = 1,   // Captured — awaiting verification
            },
            new()
            {
                TestResultId    = 12,
                TestRequestId   = 3,
                TestTypeId      = 3,
                ResultData      = "280",   // high
                Notes           = "Initial result - requires second sample for confirmation.",
                CapturedBy      = "K. Sithole",
                CapturedAt      = DateTime.Now.AddDays(-1),
                ResultStatusLookupId = 3,   // To Be Reviewed - retest needed
            },
        };

        // ── Rejection logs ─────────────────────────────────────

        public static List<StubRejectionLog> RejectionLogs { get; } = new()
        {
            new()
            {
                RejectionLogId          = 1,
                TestResultId            = 1,
                RejectedBy              = "S. Dlamini",
                RejectionNotes          = "Result value seems inconsistent with clinical picture. Please re-run.",
                PreviousResultData      = "9.8",
                PreviousTechnicianNotes = "First draw, slight haemolysis noted.",
                RejectedAt              = DateTime.Now.AddHours(-5),
            },
        };

        // ── Next ID counter (thread-unsafe but fine for demo) ──

        private static int _nextRequestId = 16;
        private static int _nextResultId = 13;
        private static int _nextRejectionId = 2;

        public static int NextRequestId() => _nextRequestId++;
        public static int NextResultId() => _nextResultId++;
        public static int NextRejectionId() => _nextRejectionId++;

        // ── Helpers ────────────────────────────────────────────

        public static StubRequestStatus? RequestStatusById(int? id) => RequestStatuses.Find(s => s.Id == id);
        public static StubResultStatus? ResultStatusById(int? id) => ResultStatuses.Find(s => s.Id == id);
        public static StubProgressStatus? ProgressStatusById(int? id) => ProgressStatuses.Find(s => s.Id == id);
        public static StubTestType? TestTypeById(int id) => TestTypes.Find(t => t.TestTypeId == id);

        public static int RequestStatusId(string name) => RequestStatuses.Find(s => s.StatusName == name)?.Id ?? 0;
        public static int ResultStatusId(string name) => ResultStatuses.Find(s => s.StatusName == name)?.Id ?? 0;
        public static int ProgressStatusId(string name) => ProgressStatuses.Find(s => s.StatusName == name)?.Id ?? 0;

        /// <summary>Hydrates navigation properties on a request from lookup lists.</summary>
        public static StubTestRequest Hydrate(StubTestRequest r)
        {
            r.TestType = TestTypeById(r.TestTypeId);
            r.RequestStatus = RequestStatusById(r.RequestStatusLookupId);
            r.TestProgressStatus = ProgressStatusById(r.TestProgressStatusLookupId);
            r.TestResults = TestResults.FindAll(tr => tr.TestRequestId == r.TestRequestId);
            r.TestResults.ForEach(tr => {
                tr.ResultStatus = ResultStatusById(tr.ResultStatusLookupId);
                tr.TestRequest = r;
            });
            return r;
        }

        /// <summary>Returns all requests hydrated, optionally filtered to a technician's assigned types.</summary>
        public static List<StubTestRequest> RequestsForTechnician(string userId)
        {
            var assignedTypeIds = TechnicianAssignments
                .FindAll(a => a.TechnicianUserId == userId || userId == "demo-tech")
                .ConvertAll(a => a.TestTypeId);

            return TestRequests
                .FindAll(r => assignedTypeIds.Contains(r.TestTypeId))
                .ConvertAll(Hydrate);
        }

        /// <summary>Returns all results for a technician's assigned test types, hydrated.</summary>
        public static List<StubTestResult> ResultsForTechnician(string userId)
        {
            var assignedTypeIds = TechnicianAssignments
                .FindAll(a => a.TechnicianUserId == userId || userId == "demo-tech")
                .ConvertAll(a => a.TestTypeId);

            return TestResults
                .FindAll(r => assignedTypeIds.Contains(r.TestTypeId))
                .ConvertAll(r => {
                    r.ResultStatus = ResultStatusById(r.ResultStatusLookupId);
                    r.TestRequest = TestRequests.Find(req => req.TestRequestId == r.TestRequestId) is { } req
                                     ? Hydrate(req) : null;
                    return r;
                });
        }
    }
}