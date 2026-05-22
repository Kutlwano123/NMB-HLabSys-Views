using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NMB_HLabSys_Views.Controllers
{
    //[Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly ILogger<DoctorController> _logger;

        // Static multi-entity mock data layers (simulating the DB structures locally)
        private static readonly List<DoctorProfile> MockDoctors = new()
        {
            new DoctorProfile { DoctorID = 1, UserID = 101, Name = "Unathi", Surname = "Mzathu", ContactNumber = "+27 82 123 4567" }
        };

        private static readonly List<UserProfile> MockUsers = new()
        {
            new UserProfile { UserID = 101, Email = "doctor@clinic.co.za", PasswordHash = "HASHED_PASSWORD" }
        };

        private static readonly List<PatientRecord> MockPatients = new()
        {
            new PatientRecord { PatientID = 1, Name = "Jabu", Surname = "Khumalo", SAIdNumber = "8503125123088", CellphoneNumber = "+27 71 987 6543", Email = "jabu@gmail.com" },
            new PatientRecord { PatientID = 2, Name = "Sarah", Surname = "Smith", SAIdNumber = "9211040124083", CellphoneNumber = "+27 63 456 7890", Email = "sarah.smith@yahoo.com" },
            new PatientRecord { PatientID = 3, Name = "Zama", Surname = "Ndlovu", SAIdNumber = "7807195088084", CellphoneNumber = "+27 84 321 0987", Email = "zama@outlook.com" }
        };

        private static readonly List<TestTypeRecord> MockTestTypes = new()
        {
            new TestTypeRecord { TestTypeID = 1, TestName = "Full Blood Count (FBC)", UnitOfMeasurement = "10^9/L" },
            new TestTypeRecord { TestTypeID = 2, TestName = "Hemoglobin (Hb)", UnitOfMeasurement = "g/dL" },
            new TestTypeRecord { TestTypeID = 3, TestName = "Platelet Count", UnitOfMeasurement = "10^9/L" },
            new TestTypeRecord { TestTypeID = 4, TestName = "White Blood Cell (WBC)", UnitOfMeasurement = "10^9/L" }
        };

        private static readonly List<TestMockRequest> MockTestRequests = new()
        {
            new TestMockRequest
            {
                TestRequestID = 1, RequestNumber = "REQ00412", PatientID = 1, DoctorID = 1,
                RequestDate = DateTime.UtcNow.AddDays(-1), Status = "Completed", CreatedAt = DateTime.UtcNow.AddDays(-1),
                Urgency = "Routine", ClinicalNotes = "Routine review data tracker.",
                Items = new List<TestMockItem>
                {
                    new TestMockItem { TestTypeID = 1, ResultValue = "4.8", RefLow = 4.0, RefHigh = 11.0, IsAbnormal = false, VerifiedBy = "Kabelo Mabidikama (Lab Tech)", ResultDate = DateTime.UtcNow.AddDays(-1) },
                    new TestMockItem { TestTypeID = 2, ResultValue = "13.2", RefLow = 12.0, RefHigh = 16.0, IsAbnormal = false, VerifiedBy = "Kabelo Mabidikama (Lab Tech)", ResultDate = DateTime.UtcNow.AddDays(-1) }
                }
            },
            new TestMockRequest
            {
                TestRequestID = 2, RequestNumber = "REQ00489", PatientID = 2, DoctorID = 1,
                RequestDate = DateTime.UtcNow, Status = "Submitted", CreatedAt = DateTime.UtcNow,
                Urgency = "Urgent", ClinicalNotes = "Patient complains of fatigue.",
                Items = new List<TestMockItem>
                {
                    new TestMockItem { TestTypeID = 2, ResultValue = "9.1", RefLow = 12.0, RefHigh = 16.0, IsAbnormal = true, VerifiedBy = "Kabelo Mabidikama (Lab Tech)", ResultDate = DateTime.UtcNow }
                }
            }
        };

        private static readonly List<MockAlert> MockAlerts = new()
        {
            new MockAlert { Id = 1, DoctorId = 1, RequestNumber = "REQ00489", PatientName = "Sarah Smith", TestName = "Hemoglobin (Hb)", Value = "9.1 g/dL", Range = "12.0 - 16.0", CreatedAt = DateTime.UtcNow }
        };

        public DoctorController(ILogger<DoctorController> logger)
        {
            _logger = logger;
        }

        private DoctorProfile GetCurrentDoctor()
        {
            return MockDoctors.First();
        }

        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        // 1. DASHBOARD (Uses a dictionary of counts alongside an inline anonymous data list)
        public IActionResult Dashboard()
        {
            var doctor = GetCurrentDoctor();
            var today = DateTime.UtcNow.Date;

            ViewBag.Stats = new Dictionary<string, int>
            {
                { "SubmittedToday", MockTestRequests.Count(r => r.RequestDate.Date == today && r.DoctorID == doctor.DoctorID) },
                { "PendingRelease", MockTestRequests.Count(r => r.Status == "Completed" && r.DoctorID == doctor.DoctorID) },
                { "AbnormalAlerts", MockAlerts.Count(a => a.DoctorId == doctor.DoctorID && a.CreatedAt >= DateTime.UtcNow.AddDays(-5)) },
                { "NotificationsSent", 2 }
            };

            // Project a direct list of localized tracking data definitions straight to the view
            var recentRequests = MockTestRequests
                .Where(r => r.DoctorID == doctor.DoctorID)
                .OrderByDescending(r => r.CreatedAt)
                .Take(6)
                .Select(r => {
                    var p = MockPatients.FirstOrDefault(pt => pt.PatientID == r.PatientID);
                    return (
                        Id: r.TestRequestID,
                        RequestNumber: r.RequestNumber,
                        PatientName: p != null ? $"{p.Name} {p.Surname}" : "Unknown",
                        Status: r.Status,
                        UpdatedAt: r.CreatedAt
                    );
                }).ToList();

            return View(recentRequests);
        }

        // 2. MANAGE PATIENT RECORDS (Passes the flat list of records directly)
        public IActionResult Patients()
        {
            var patients = MockPatients
                .OrderBy(p => p.Surname).ThenBy(p => p.Name)
                .ToList();

            return View(patients);
        }

        // 3. CREATE TEST REQUESTS (GET - Utilizes ViewBag for select lists)
        [HttpGet]
        public IActionResult CreateRequest()
        {
            ViewBag.PatientOptions = MockPatients
                .OrderBy(p => p.Surname)
                .Select(p => new SelectListItem
                {
                    Value = p.PatientID.ToString(),
                    Text = $"{p.Surname}, {p.Name} ({p.SAIdNumber})"
                }).ToList();

            ViewBag.TestTypeOptions = MockTestTypes
                .OrderBy(t => t.TestName)
                .Select(t => new SelectListItem
                {
                    Value = t.TestTypeID.ToString(),
                    Text = t.TestName
                }).ToList();

            return View();
        }

        // 4. CREATE TEST REQUESTS (POST - Collects data directly from primitive parameters)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRequest(int? patientId, string urgency, string? clinicalNotes, string? rawBarcodesInput, List<int> selectedTestTypeIds)
        {
            if (patientId == null || selectedTestTypeIds == null || !selectedTestTypeIds.Any())
            {
                ModelState.AddModelError("", "Please select a patient and at least one test parameter.");

                // Repopulate options bags
                ViewBag.PatientOptions = MockPatients.OrderBy(p => p.Surname).Select(p => new SelectListItem { Value = p.PatientID.ToString(), Text = $"{p.Surname}, {p.Name}" }).ToList();
                ViewBag.TestTypeOptions = MockTestTypes.OrderBy(t => t.TestName).Select(t => new SelectListItem { Value = t.TestTypeID.ToString(), Text = t.TestName }).ToList();

                return View();
            }

            var doctor = GetCurrentDoctor();

            var newRequest = new TestMockRequest
            {
                TestRequestID = MockTestRequests.Count + 1,
                RequestNumber = "REQ" + new Random().Next(10000, 99999),
                PatientID = patientId.Value,
                RequestDate = DateTime.UtcNow,
                Urgency = urgency ?? "Routine",
                ClinicalNotes = clinicalNotes,
                Status = "Submitted",
                CreatedAt = DateTime.UtcNow,
                DoctorID = doctor.DoctorID,
                Items = selectedTestTypeIds.Select(id => new TestMockItem
                {
                    TestTypeID = id,
                    ResultValue = "Pending",
                    IsAbnormal = false
                }).ToList()
            };

            MockTestRequests.Add(newRequest);

            TempData["Success"] = "Test request created successfully.";
            return RedirectToAction(nameof(TrackRequests));
        }

        // 5. TRACK TEST REQUEST STATUS (Uses a strong inline Tuple structure to map values cleanly into table rows)
        public IActionResult TrackRequests()
        {
            var doctor = GetCurrentDoctor();

            var requests = MockTestRequests
                .Where(r => r.DoctorID == doctor.DoctorID)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => {
                    var p = MockPatients.FirstOrDefault(pt => pt.PatientID == r.PatientID);
                    return (
                        Id: r.TestRequestID,
                        RequestNumber: r.RequestNumber,
                        PatientName: p != null ? $"{p.Name} {p.Surname}" : "Unknown",
                        Status: r.Status,
                        UpdatedAt: r.CreatedAt
                    );
                }).ToList();

            return View("TrackRequests", requests);
        }

        // 6. VIEW RESULTS (Returns the single matching request directly)
        public IActionResult Results(int? id)
        {
            if (id == null || id.Value == 0)
            {
                ViewBag.RequestFound = false;
                return View();
            }

            var request = MockTestRequests.FirstOrDefault(r => r.TestRequestID == id.Value);
            if (request == null) return NotFound();

            var patient = MockPatients.FirstOrDefault(p => p.PatientID == request.PatientID);

            ViewBag.RequestFound = true;
            ViewBag.RequestId = request.TestRequestID;
            ViewBag.RequestNumber = request.RequestNumber;
            ViewBag.PatientName = patient != null ? $"{patient.Name} {patient.Surname}" : "Unknown";
            ViewBag.Status = request.Status;

            // Send a collection of item dictionaries containing joined information down to the loop framework
            var resultsData = request.Items.Select(i => {
                var tt = MockTestTypes.FirstOrDefault(t => t.TestTypeID == i.TestTypeID);
                return new Dictionary<string, object>
                {
                    { "TestName", tt?.TestName ?? "Unknown" },
                    { "Value", i.ResultValue },
                    { "Units", tt?.UnitOfMeasurement ?? string.Empty },
                    { "RefLow", i.RefLow },
                    { "RefHigh", i.RefHigh },
                    { "IsAbnormal", i.IsAbnormal },
                    { "VerifiedBy", i.VerifiedBy ?? "In Queue" },
                    { "VerifiedAt", i.ResultDate ?? (object)"—" }
                };
            }).ToList();

            return View(resultsData);
        }

        // 7. RELEASE RESULTS WITH NOTES
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReleaseResults(int requestId, string? note)
        {
            var request = MockTestRequests.FirstOrDefault(r => r.TestRequestID == requestId);
            if (request == null) return NotFound();

            if (request.Status != "Completed")
            {
                TempData["Error"] = "Only completed laboratory requests can be released.";
                return RedirectToAction(nameof(Results), new { id = requestId });
            }

            request.Status = "Released";
            TempData["Success"] = "Results released to patient.";
            return RedirectToAction(nameof(Results), new { id = requestId });
        }

        // 8. VIEW ALERTS
        public IActionResult Alerts(DateTime? from = null, DateTime? to = null)
        {
            var doctor = GetCurrentDoctor();
            var end = to?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow;
            var start = from?.Date ?? DateTime.UtcNow.AddDays(-5);

            var filteredAlerts = MockAlerts
                .Where(a => a.DoctorId == doctor.DoctorID && a.CreatedAt >= start && a.CreatedAt <= end)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            return View(filteredAlerts);
        }

        // 9. DOCTOR REPORTS
        public IActionResult Reports()
        {
            ViewBag.FromDate = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
            ViewBag.ToDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var previewRequests = MockTestRequests.Select(r => {
                var p = MockPatients.FirstOrDefault(pt => pt.PatientID == r.PatientID);
                return (
                    RequestNumber: r.RequestNumber,
                    PatientName: p != null ? $"{p.Name} {p.Surname}" : "Unknown",
                    Status: r.Status,
                    UpdatedAt: r.CreatedAt
                );
            }).ToList();

            return View(previewRequests);
        }

        // 10. DOCTOR SETTINGS
        [HttpGet]
        public IActionResult Settings()
        {
            var doctor = GetCurrentDoctor();
            var user = MockUsers.First(u => u.UserID == doctor.UserID);

            ViewBag.Email = user.Email;
            ViewBag.Phone = doctor.ContactNumber;
            ViewBag.City = "Gqeberha";
            ViewBag.PostalCode = "6001";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(string email, string phone, string city, string postalCode)
        {
            var doctor = GetCurrentDoctor();
            var user = MockUsers.First(u => u.UserID == doctor.UserID);

            user.Email = email;
            doctor.ContactNumber = phone;

            TempData["SettingsSuccess"] = "Profile updated successfully (In-Memory).";
            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(string currentPassword, string newPassword)
        {
            TempData["SettingsSuccess"] = "Password changed successfully (In-Memory).";
            return RedirectToAction(nameof(Settings));
        }
    }

    // Completely decoupled local internal data structures substituting domain records
    public class DoctorProfile { public int DoctorID { get; set; } public int UserID { get; set; } public string? Name { get; set; } public string? Surname { get; set; } public string? ContactNumber { get; set; } }
    public class UserProfile { public int UserID { get; set; } public string? Email { get; set; } public string? PasswordHash { get; set; } }
    public class PatientRecord { public int PatientID { get; set; } public string? Name { get; set; } public string? Surname { get; set; } public string? SAIdNumber { get; set; } public string? CellphoneNumber { get; set; } public string? Email { get; set; } }
    public class TestTypeRecord { public int TestTypeID { get; set; } public string? TestName { get; set; } public string? UnitOfMeasurement { get; set; } }
    public class TestMockRequest { public int TestRequestID { get; set; } public string RequestNumber { get; set; } = string.Empty; public int PatientID { get; set; } public int DoctorID { get; set; } public DateTime RequestDate { get; set; } public string Status { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } public string? Urgency { get; set; } public string? ClinicalNotes { get; set; } public List<TestMockItem> Items { get; set; } = new(); }
    public class TestMockItem { public int TestTypeID { get; set; } public string ResultValue { get; set; } = string.Empty; public double RefLow { get; set; } public double RefHigh { get; set; } public bool IsAbnormal { get; set; } public string? VerifiedBy { get; set; } public DateTime? ResultDate { get; set; } }
    public class MockAlert { public int Id { get; set; } public int DoctorId { get; set; } public string RequestNumber { get; set; } = string.Empty; public string PatientName { get; set; } = string.Empty; public string TestName { get; set; } = string.Empty; public string Value { get; set; } = string.Empty; public string Range { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } }
}