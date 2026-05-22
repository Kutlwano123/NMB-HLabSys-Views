// ============================================================
//  LabTechnicianController.cs  —  NMB_HLabSys (VIEWS)
//  Zero EF Core. Zero DbContext. Zero Identity.
//  All data comes from StubData static collections.
// ============================================================
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NMB_HLabSys_VIEWS.Data;
using NMB_HLabSys_VIEWS.ViewModels;

namespace NMB_HLabSys_VIEWS.Controllers
{
    public class LabTechnicianController : Controller
    {
        // ── Identity helpers ───────────────────────────────────
        // Falls back to "demo-tech" so every page works without login.

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "demo-tech";
        private string UserName => User.Identity?.Name ?? "Lab Technician";

        // ── Index ──────────────────────────────────────────────

        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        // ── Dashboard ──────────────────────────────────────────

        public IActionResult Dashboard()
        {
            var all = StubData.RequestsForTechnician(UserId);
            var results = StubData.ResultsForTechnician(UserId);
            var completedId = StubData.RequestStatusId(StatusNames.Completed);
            var pendingId = StubData.RequestStatusId(StatusNames.Pending);
            var samplesId = StubData.RequestStatusId(StatusNames.SamplesReceived);
            var selectedId = StubData.RequestStatusId(StatusNames.Selected);
            var capturedId = StubData.ResultStatusId(StatusNames.ResultCaptured);
            var reviewId = StubData.ResultStatusId(StatusNames.ResultToBeReviewed);

            var vm = new LabDashboardViewModel
            {
                SelectedByTechnician = all.Count(r => r.RequestStatusLookupId == selectedId && r.AssignedTechnicianUserId == UserId),
                WaitingToBeSelected = all.Count(r => r.RequestStatusLookupId == pendingId || r.RequestStatusLookupId == samplesId),
                WaitingToBeVerified = results.Count(r => r.ResultStatusLookupId == capturedId),
                WaitingToBeReviewed = results.Count(r => r.ResultStatusLookupId == reviewId && r.CapturedBy == UserName),
                UrgentTests = all.Count(r => r.Urgency == "STAT" || r.Urgency == "Urgent"),
                OverdueTests = all.Count(r => r.DueDate < DateTime.Now && r.RequestStatusLookupId != completedId),
                NearingLimit = all.Count(r => r.DueDate <= DateTime.Now.AddDays(2) && r.DueDate >= DateTime.Now && r.RequestStatusLookupId != completedId),
            };

            ViewData["ActivePage"] = "Dashboard";
            return View(vm);
        }

        // ── Pending Tests ──────────────────────────────────────

        public IActionResult PendingTests()
        {
            var pendingId = StubData.RequestStatusId(StatusNames.Pending);
            var samplesId = StubData.RequestStatusId(StatusNames.SamplesReceived);
            var list = StubData.RequestsForTechnician(UserId)
                               .Where(r => r.RequestStatusLookupId == pendingId || r.RequestStatusLookupId == samplesId)
                               .ToList();
            ViewData["ActivePage"] = "PendingTests";
            return View(list);
        }

        // ── Selected Tests ─────────────────────────────────────

        public IActionResult SelectedTests()
        {
            var selectedId = StubData.RequestStatusId(StatusNames.Selected);
            var list = StubData.RequestsForTechnician(UserId)
                               .Where(r => r.RequestStatusLookupId == selectedId && r.AssignedTechnicianUserId == UserId)
                               .ToList();
            ViewData["ActivePage"] = "SelectedTests";
            return View(list);
        }

        // ── Urgent Tests ───────────────────────────────────────

        public IActionResult UrgentTests()
        {
            var list = StubData.RequestsForTechnician(UserId)
                               .Where(r => r.Urgency == "STAT" || r.Urgency == "Urgent")
                               .ToList();
            ViewData["ActivePage"] = "UrgentTests";
            return View(list);
        }

        // ── Overdue Tests ──────────────────────────────────────

        public IActionResult OverdueTests()
        {
            var completedId = StubData.RequestStatusId(StatusNames.Completed);
            var list = StubData.RequestsForTechnician(UserId)
                               .Where(r => r.DueDate < DateTime.Now && r.RequestStatusLookupId != completedId)
                               .ToList();
            ViewData["ActivePage"] = "OverdueTests";
            return View(list);
        }

        // ── Nearing Limit ──────────────────────────────────────

        public IActionResult NearingLimit()
        {
            var completedId = StubData.RequestStatusId(StatusNames.Completed);
            var cutoff = DateTime.Now.AddDays(2);
            var list = StubData.RequestsForTechnician(UserId)
                               .Where(r => r.DueDate <= cutoff && r.DueDate >= DateTime.Now && r.RequestStatusLookupId != completedId)
                               .ToList();
            ViewData["ActivePage"] = "NearingLimit";
            return View("TestRequests", list);
        }

        // ── All Test Requests ──────────────────────────────────

        public IActionResult TestRequests()
        {
            ViewData["ActivePage"] = "TestRequests";
            return View(StubData.RequestsForTechnician(UserId));
        }

        // ── Tasks ──────────────────────────────────────────────

        public IActionResult Tasks()
        {
            var completedId = StubData.RequestStatusId(StatusNames.Completed);
            var list = StubData.RequestsForTechnician(UserId)
                               .Where(r => r.RequestStatusLookupId != completedId)
                               .ToList();
            ViewData["ActivePage"] = "Tasks";
            return View(list);
        }

        // ── Test Results ───────────────────────────────────────

        public IActionResult TestResults()
        {
            ViewData["ActivePage"] = "TestResults";
            return View(StubData.ResultsForTechnician(UserId));
        }

        // ── Verify Tests ───────────────────────────────────────

        public IActionResult VerifyTests()
        {
            var capturedId = StubData.ResultStatusId(StatusNames.ResultCaptured);
            var list = StubData.ResultsForTechnician(UserId)
                               .Where(r => r.ResultStatusLookupId == capturedId)
                               .ToList();
            ViewData["ActivePage"] = "VerifyTests";
            return View("TestResults", list);
        }

        // ── Review Tests ───────────────────────────────────────

        public IActionResult ReviewTests()
        {
            var reviewId = StubData.ResultStatusId(StatusNames.ResultToBeReviewed);
            var list = StubData.ResultsForTechnician(UserId)
                               .Where(r => r.ResultStatusLookupId == reviewId && r.CapturedBy == UserName)
                               .ToList();
            ViewData["ActivePage"] = "ReviewTests";
            return View("TestResults", list);
        }

        // ── Receive Samples (POST) ─────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReceiveSamples(int requestId)
        {
            var req = StubData.TestRequests.Find(r => r.TestRequestId == requestId);
            if (req == null) return NotFound();

            req.RequestStatusLookupId = StubData.RequestStatusId(StatusNames.SamplesReceived);
            req.TestProgressStatusLookupId = StubData.ProgressStatusId(StatusNames.ProgressSubmitted);

            TempData["Success"] = "Samples received. Request status updated.";
            return RedirectToAction(nameof(PendingTests));
        }

        // ── Select Test (POST) ─────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectTest(int requestId)
        {
            var req = StubData.TestRequests.Find(r => r.TestRequestId == requestId);
            if (req == null) return NotFound();

            req.AssignedTechnicianUserId = UserId;
            req.RequestStatusLookupId = StubData.RequestStatusId(StatusNames.Selected);
            req.TestProgressStatusLookupId = StubData.ProgressStatusId(StatusNames.ProgressInProgress);

            TempData["Success"] = "Test selected. (Stub: no consumables deducted in demo mode.)";
            return RedirectToAction(nameof(SelectedTests));
        }

        // ── Capture Result Form (GET) ──────────────────────────

        [HttpGet]
        public IActionResult CaptureResultForm(int requestId)
        {
            var raw = StubData.TestRequests.Find(r => r.TestRequestId == requestId);
            if (raw == null) return NotFound();

            var request = StubData.Hydrate(raw);
            var tt = request.TestType;

            var vm = new ResultCaptureViewModel
            {
                TestRequestId = request.TestRequestId,
                RequestNumber = request.RequestNumber,
                PatientName = request.PatientName,
                TestName = tt?.TestName ?? "Assay",
                UnitOfMeasurement = tt?.UnitOfMeasurement ?? "g/dL",
                MinNormalRange = double.TryParse(tt?.MinNormalRange, out var mn) ? mn : 0,
                MaxNormalRange = double.TryParse(tt?.MaxNormalRange, out var mx) ? mx : 0,
                ClinicalNotes = request.ClinicalNotes ?? "No clinical notes.",
                MedicalConditions = SplitSnapshot(request.PatientConditionsSnapshot),
                Allergies = SplitSnapshot(request.PatientAllergiesSnapshot),
                CurrentMedications = SplitSnapshot(request.PatientMedicationsSnapshot),
                IsOverdue = request.IsOverdue,
            };

            ViewData["ActivePage"] = "Tasks";
            return View(vm);
        }

        // ── Capture Result (POST) ──────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CaptureResult(ResultCaptureViewModel model)
        {
            if (!ModelState.IsValid) return View("CaptureResultForm", model);

            var req = StubData.TestRequests.Find(r => r.TestRequestId == model.TestRequestId);
            if (req == null) return NotFound();

            // Add result to in-memory store
            var result = new StubTestResult
            {
                TestResultId = StubData.NextResultId(),
                TestRequestId = model.TestRequestId,
                TestTypeId = req.TestTypeId,
                ResultData = model.ResultData ?? "",
                Notes = model.TechnicianNotes ?? "",
                CapturedBy = UserName,
                CapturedAt = DateTime.Now,
                ResultStatusLookupId = StubData.ResultStatusId(StatusNames.ResultCaptured),
            };

            StubData.TestResults.Add(result);

            req.RequestStatusLookupId = StubData.RequestStatusId(StatusNames.InProgress);
            req.TestProgressStatusLookupId = StubData.ProgressStatusId(StatusNames.ProgressCompleted);

            TempData["Success"] = "Results captured. Awaiting peer verification.";
            return RedirectToAction(nameof(TestResults));
        }

        // ── Verify Result Form (GET) ───────────────────────────

        [HttpGet]
        public IActionResult VerifyResultForm(int resultId)
        {
            var res = StubData.TestResults.Find(r => r.TestResultId == resultId);
            if (res == null) return NotFound();

            var req = StubData.TestRequests.Find(r => r.TestRequestId == res.TestRequestId);
            var tt = req != null ? StubData.TestTypeById(req.TestTypeId) : null;

            var minVal = double.TryParse(tt?.MinNormalRange, out var mn) ? mn : 0;
            var maxVal = double.TryParse(tt?.MaxNormalRange, out var mx) ? mx : 0;

            var vm = new ResultVerificationViewModel
            {
                TestResultId = res.TestResultId,
                RequestNumber = req?.RequestNumber,
                PatientName = req?.PatientName,
                TestName = tt?.TestName ?? "Assay",
                CapturedByTechnician = res.CapturedBy,
                ResultData = res.ResultData,
                MinNormalRange = minVal,
                MaxNormalRange = maxVal,
                UnitOfMeasurement = tt?.UnitOfMeasurement ?? "",
                IsOutOfRange = IsOutOfRange(res.ResultData, minVal, maxVal),
            };

            ViewData["ActivePage"] = "VerifyTests";
            return View(vm);
        }

        // ── Approve Result (POST) ──────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveResult(int resultId)
        {
            var res = StubData.TestResults.Find(r => r.TestResultId == resultId);
            if (res == null) return NotFound();

            if (string.Equals(res.CapturedBy, UserName, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "You cannot verify your own results. A different technician must approve.";
                return RedirectToAction(nameof(VerifyTests));
            }

            res.ResultStatusLookupId = StubData.ResultStatusId(StatusNames.ResultVerified);
            res.VerifiedBy = UserName;
            res.VerifiedAt = DateTime.Now;

            var req = StubData.TestRequests.Find(r => r.TestRequestId == res.TestRequestId);
            if (req != null)
            {
                req.TestProgressStatusLookupId = StubData.ProgressStatusId(StatusNames.ProgressVerified);
                req.RequestStatusLookupId = StubData.RequestStatusId(StatusNames.Completed);
            }

            TempData["Success"] = "Result verified successfully.";
            return RedirectToAction(nameof(VerifyTests));
        }

        // ── Reject Result (POST) ───────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectResult(int resultId, string validationNotes)
        {
            if (string.IsNullOrWhiteSpace(validationNotes))
            {
                TempData["Error"] = "Rejection notes are required.";
                return RedirectToAction(nameof(VerifyResultForm), new { resultId });
            }

            var res = StubData.TestResults.Find(r => r.TestResultId == resultId);
            if (res == null) return NotFound();

            if (string.Equals(res.CapturedBy, UserName, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "You cannot reject your own results.";
                return RedirectToAction(nameof(VerifyTests));
            }

            StubData.RejectionLogs.Add(new StubRejectionLog
            {
                RejectionLogId = StubData.NextRejectionId(),
                TestResultId = res.TestResultId,
                RejectedBy = UserName,
                RejectionNotes = validationNotes,
                PreviousResultData = res.ResultData,
                PreviousTechnicianNotes = res.Notes,
                RejectedAt = DateTime.Now,
            });

            res.ResultStatusLookupId = StubData.ResultStatusId(StatusNames.ResultToBeReviewed);
            res.Notes = validationNotes;

            var req = StubData.TestRequests.Find(r => r.TestRequestId == res.TestRequestId);
            if (req != null)
                req.TestProgressStatusLookupId = StubData.ProgressStatusId(StatusNames.ProgressToBeReviewed);

            TempData["Success"] = "Result rejected. Capturing technician may re-capture.";
            return RedirectToAction(nameof(ReviewTests));
        }

        // ── Rejection History ──────────────────────────────────

        public IActionResult RejectionHistory(int resultId)
        {
            var logs = StubData.RejectionLogs
                               .Where(l => l.TestResultId == resultId)
                               .OrderByDescending(l => l.RejectedAt)
                               .ToList();
            ViewData["ActivePage"] = "TestResults";
            return View(logs);
        }

        // ── Productivity Report ────────────────────────────────

        public IActionResult ProductivityReport()
        {
            var requests = StubData.RequestsForTechnician(UserId);
            var html = BuildProductivityHtml(requests);
            var bytes = System.Text.Encoding.UTF8.GetBytes(html);
            return File(bytes, "text/html", $"productivity-report-{DateTime.Now:yyyyMMdd}.html");
        }

        // ── Helpers ────────────────────────────────────────────

        private static bool IsOutOfRange(string? value, double min, double max)
        {
            if (!double.TryParse(value, out var v)) return false;
            return v < min || v > max;
        }

        // ── Settings ───────────────────────────────────────────

        public IActionResult Settings()
        {
            ViewBag.DisplayName = HttpContext.Session.GetString("TechDisplayName") ?? UserName;
            ViewBag.Email = HttpContext.Session.GetString("TechEmail") ?? User.Identity?.Name ?? "technician@nmbhdl.co.za";
            ViewBag.Phone = HttpContext.Session.GetString("TechPhone") ?? "";
            ViewBag.Specialty = HttpContext.Session.GetString("TechSpecialty") ?? "";
            ViewBag.Notes = HttpContext.Session.GetString("TechNotes") ?? "";
            ViewBag.LastUpdated = HttpContext.Session.GetString("TechLastUpdated");
            ViewData["ActivePage"] = "Settings";
            return View();
        }

        [HttpPost]
        public IActionResult UpdateSettings(string displayName, string email, string phone, string specialty, string notes)
        {
            // Store settings in session (mocked persistence)
            HttpContext.Session.SetString("TechDisplayName", displayName ?? "");
            HttpContext.Session.SetString("TechEmail", email ?? "");
            HttpContext.Session.SetString("TechPhone", phone ?? "");
            HttpContext.Session.SetString("TechSpecialty", specialty ?? "");
            HttpContext.Session.SetString("TechNotes", notes ?? "");
            HttpContext.Session.SetString("TechLastUpdated", DateTime.Now.ToString("g"));

            TempData["Success"] = "Your profile settings have been updated successfully.";
            return RedirectToAction(nameof(Settings));
        }

        private static List<string> SplitSnapshot(string? value) =>
            string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        private static string BuildProductivityHtml(List<StubTestRequest> requests)
        {
            var rows = string.Join("", requests.Select(r =>
                $"<tr><td>{r.RequestNumber}</td><td>{r.PatientName}</td><td>{r.TestType?.TestName}</td>" +
                $"<td>{r.Urgency}</td><td>{r.RequestDate:g}</td><td>{r.RequestStatus?.StatusName}</td></tr>"));

            return $@"<!DOCTYPE html><html><head><meta charset='utf-8'>
<title>Productivity Report</title>
<style>body{{font-family:sans-serif;padding:32px}}table{{border-collapse:collapse;width:100%}}
th,td{{border:1px solid #ccc;padding:8px 12px;text-align:left}}th{{background:#7a3030;color:#fff}}</style>
</head><body>
<h2>Lab Technician Productivity Report — {DateTime.Now:dd MMM yyyy}</h2>
<table><thead><tr><th>Request #</th><th>Patient</th><th>Test</th><th>Urgency</th><th>Requested</th><th>Status</th></tr></thead>
<tbody>{rows}</tbody></table>
</body></html>";
        }
    }
}