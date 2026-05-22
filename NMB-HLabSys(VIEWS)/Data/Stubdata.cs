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

        private static int _nextRequestId = 6;
        private static int _nextResultId = 3;
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