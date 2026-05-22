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

            // Generate notifications
            StubData.GenerateNotificationsForTechnician(UserId);

            var vm = new LabDashboardViewModel
            {
                SelectedByTechnician = all.Count(r => r.RequestStatusLookupId == selectedId && r.AssignedTechnicianUserId == UserId),
                WaitingToBeSelected = all.Count(r => r.RequestStatusLookupId == pendingId || r.RequestStatusLookupId == samplesId),
                WaitingToBeVerified = results.Count(r => r.ResultStatusLookupId == capturedId),
                WaitingToBeReviewed = results.Count(r => r.ResultStatusLookupId == reviewId && r.CapturedBy == UserName),
                UrgentTests = all.Count(r => r.Urgency == "STAT" || r.Urgency == "Urgent"),
                OverdueTests = all.Count(r => r.DueDate < DateTime.Now && r.RequestStatusLookupId != completedId),
                NearingLimit = all.Count(r => r.DueDate <= DateTime.Now.AddDays(2) && r.DueDate >= DateTime.Now && r.RequestStatusLookupId != completedId),

                // Add recent notifications (top 5)
                RecentNotifications = StubData.AllNotificationsForTechnician(UserId)
                    .Take(5)
                    .ToList()
                    .ConvertAll(n => new NotificationViewModel
                    {
                        NotificationId = n.NotificationId,
                        Title = n.Title,
                        Message = n.Message,
                        NotificationType = n.NotificationType,
                        CreatedAt = n.CreatedAt,
                        IsRead = n.IsRead,
                        RelatedTestRequestId = n.RelatedTestRequestId,
                    }),
                UnreadNotificationCount = StubData.UnreadNotificationsForTechnician(UserId).Count,

                // Add critical consumables (low stock)
                CriticalConsumables = StubData.ConsumablesForTechnician(UserId)
                    .Where(c => c.QuantityInStock <= c.MinimumThreshold)
                    .ToList()
                    .ConvertAll(c => new ConsumableInventoryViewModel
                    {
                        ConsumableId = c.ConsumableId,
                        ConsumableName = c.ConsumableName,
                        Unit = c.Unit,
                        QuantityInStock = c.QuantityInStock,
                        MinimumThreshold = c.MinimumThreshold,
                    }),
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
        public IActionResult CaptureResultForm(int requestId, bool isRecapture = false)
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
                IsRecapture = isRecapture,
            };

            // Load required consumables for this test type
            var consumableReqs = StubData.ConsumablesForTestType(request.TestTypeId);
            vm.RequiredConsumables = consumableReqs.ConvertAll(r => new ConsumableRequirementViewModel
            {
                RequirementId = r.RequirementId,
                TestTypeId = r.TestTypeId,
                TestName = r.TestType?.TestName ?? "",
                ConsumableId = r.ConsumableId,
                ConsumableName = r.Consumable?.ConsumableName ?? "",
                QuantityRequired = r.QuantityRequired,
                Unit = r.Consumable?.Unit ?? "",
                AvailableQuantity = r.Consumable?.QuantityInStock ?? 0,
            });

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

        // ══════════════════════════════════════════════════════
        //  NOTIFICATIONS
        // ══════════════════════════════════════════════════════

        /// <summary>Display notification center with all notifications.</summary>
        public IActionResult Notifications()
        {
            StubData.GenerateNotificationsForTechnician(UserId);
            var notifs = StubData.AllNotificationsForTechnician(UserId);

            var vm = new NotificationCenterViewModel
            {
                Notifications = notifs.ConvertAll(n => new NotificationViewModel
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    NotificationType = n.NotificationType,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                    RelatedTestRequestId = n.RelatedTestRequestId,
                    RelatedTestResultId = n.RelatedTestResultId,
                }),
                UnreadCount = StubData.UnreadNotificationsForTechnician(UserId).Count,
            };

            ViewData["ActivePage"] = "Notifications";
            return View(vm);
        }

        /// <summary>Mark a single notification as read (AJAX).</summary>
        [HttpPost]
        public IActionResult MarkNotificationRead(int notificationId)
        {
            StubData.MarkNotificationAsRead(notificationId);
            return Json(new { success = true });
        }

        /// <summary>Get unread notification count (AJAX).</summary>
        [HttpGet]
        public IActionResult GetUnreadNotificationCount()
        {
            var count = StubData.UnreadNotificationsForTechnician(UserId).Count;
            return Json(new { unreadCount = count });
        }

        // ══════════════════════════════════════════════════════
        //  CONSUMABLES
        // ══════════════════════════════════════════════════════

        /// <summary>Display consumable inventory for technician's assigned tests.</summary>
        public IActionResult ConsumableInventory()
        {
            var consumables = StubData.ConsumablesForTechnician(UserId);

            var vm = new List<ConsumableInventoryViewModel>();
            foreach (var c in consumables)
            {
                vm.Add(new ConsumableInventoryViewModel
                {
                    ConsumableId = c.ConsumableId,
                    ConsumableName = c.ConsumableName,
                    Unit = c.Unit,
                    QuantityInStock = c.QuantityInStock,
                    MinimumThreshold = c.MinimumThreshold,
                });
            }

            ViewData["ActivePage"] = "Consumables";
            return View(vm.OrderBy(c => c.IsLow ? 0 : 1).ToList());
        }

        /// <summary>Display consumables required for a specific test before capture.</summary>
        [HttpGet]
        public IActionResult RequiredConsumables(int testTypeId)
        {
            var reqs = StubData.ConsumablesForTestType(testTypeId);
            var testType = StubData.TestTypeById(testTypeId);

            var vm = new ConsumableTrackingViewModel
            {
                TestTypeId = testTypeId,
                TestName = testType?.TestName ?? "Unknown Test",
                Requirements = reqs.ConvertAll(r => new ConsumableRequirementViewModel
                {
                    RequirementId = r.RequirementId,
                    TestTypeId = r.TestTypeId,
                    TestName = r.TestType?.TestName ?? "",
                    ConsumableId = r.ConsumableId,
                    ConsumableName = r.Consumable?.ConsumableName ?? "",
                    QuantityRequired = r.QuantityRequired,
                    Unit = r.Consumable?.Unit ?? "",
                    AvailableQuantity = r.Consumable?.QuantityInStock ?? 0,
                }),
                AllConsumablesAvailable = reqs.All(r => (r.Consumable?.QuantityInStock ?? 0) >= r.QuantityRequired),
            };

            ViewData["ActivePage"] = "Tasks";
            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        //  RECAPTURE & REJECTION FILTERING
        // ══════════════════════════════════════════════════════

        /// <summary>Display list of rejected tests for recapture.</summary>
        public IActionResult RejectedForRecapture()
        {
            var reviewId = StubData.ResultStatusId(StatusNames.ResultToBeReviewed);
            var results = StubData.ResultsForTechnician(UserId)
                                 .Where(r => r.ResultStatusLookupId == reviewId && r.CapturedBy == UserName)
                                 .ToList();

            var vm = new List<RejectionHistoryViewModel>();
            foreach (var res in results)
            {
                var rejection = StubData.RejectionLogs
                    .Where(l => l.TestResultId == res.TestResultId)
                    .OrderByDescending(l => l.RejectedAt)
                    .FirstOrDefault();

                var req = res.TestRequest ?? StubData.TestRequests.Find(r => r.TestRequestId == res.TestRequestId);

                vm.Add(new RejectionHistoryViewModel
                {
                    RejectionLogId = rejection?.RejectionLogId ?? 0,
                    TestResultId = res.TestResultId,
                    TestRequestId = res.TestRequestId,
                    RequestNumber = req?.RequestNumber ?? "",
                    PatientName = req?.PatientName ?? "",
                    TestName = req?.TestType?.TestName ?? "",
                    RejectedBy = rejection?.RejectedBy ?? "",
                    RejectionNotes = rejection?.RejectionNotes ?? "",
                    PreviousResultData = rejection?.PreviousResultData ?? "",
                    PreviousTechnicianNotes = rejection?.PreviousTechnicianNotes ?? "",
                    RejectedAt = rejection?.RejectedAt ?? DateTime.Now,
                    CanRecapture = true,
                });
            }

            ViewData["ActivePage"] = "ReviewTests";
            return View(vm);
        }

        /// <summary>Show recapture form with pre-populated previous data.</summary>
        [HttpGet]
        public IActionResult RecaptureForm(int testResultId)
        {
            var result = StubData.TestResults.Find(r => r.TestResultId == testResultId);
            if (result == null) return NotFound();

            var request = StubData.TestRequests.Find(r => r.TestRequestId == result.TestRequestId);
            if (request == null) return NotFound();

            request = StubData.Hydrate(request);
            var testType = request.TestType;

            var rejection = StubData.RejectionLogs
                .Where(l => l.TestResultId == testResultId)
                .OrderByDescending(l => l.RejectedAt)
                .FirstOrDefault();

            var vm = new RecaptureFormViewModel
            {
                TestResultId = testResultId,
                TestRequestId = request.TestRequestId,
                RequestNumber = request.RequestNumber,
                PatientName = request.PatientName,
                TestName = testType?.TestName ?? "",
                UnitOfMeasurement = testType?.UnitOfMeasurement ?? "",
                MinNormalRange = double.TryParse(testType?.MinNormalRange, out var mn) ? mn : 0,
                MaxNormalRange = double.TryParse(testType?.MaxNormalRange, out var mx) ? mx : 0,
                ClinicalNotes = request.ClinicalNotes ?? "",
                RejectedBy = rejection?.RejectedBy ?? "",
                RejectionNotes = rejection?.RejectionNotes ?? "",
                PreviousResultData = rejection?.PreviousResultData ?? result.ResultData,
                PreviousTechnicianNotes = rejection?.PreviousTechnicianNotes ?? result.Notes,
                MedicalConditions = SplitSnapshot(request.PatientConditionsSnapshot),
                Allergies = SplitSnapshot(request.PatientAllergiesSnapshot),
                CurrentMedications = SplitSnapshot(request.PatientMedicationsSnapshot),
            };

            ViewData["ActivePage"] = "ReviewTests";
            return View(vm);
        }

        /// <summary>Submit recaptured result.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitRecapture(RecaptureFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("RecaptureForm", model);

            var oldResult = StubData.TestResults.Find(r => r.TestResultId == model.TestResultId);
            if (oldResult == null) return NotFound();

            // Update old result or create new one (in production, you'd handle versioning)
            oldResult.ResultData = model.NewResultData ?? "";
            oldResult.Notes = model.NewTechnicianNotes ?? "";
            oldResult.CapturedBy = UserName;
            oldResult.CapturedAt = DateTime.Now;
            oldResult.ResultStatusLookupId = StubData.ResultStatusId(StatusNames.ResultCaptured);

            TempData["Success"] = "Result recaptured successfully. Awaiting peer verification.";
            return RedirectToAction(nameof(RejectedForRecapture));
        }

        // ══════════════════════════════════════════════════════
        //  TABLE FILTERING
        // ══════════════════════════════════════════════════════

        /// <summary>Display all tests with advanced filtering options.</summary>
        [HttpGet]
        public IActionResult FilteredTests(string? searchTerm = null, string? urgencyFilter = null,
                                           string? statusFilter = null, string? categoryFilter = null,
                                           DateTime? fromDate = null, DateTime? toDate = null,
                                           string? sortBy = "DueDate", string? sortOrder = "asc")
        {
            var all = StubData.RequestsForTechnician(UserId);

            // Apply filters
            var filtered = all.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                filtered = filtered.Where(r => r.RequestNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                               r.PatientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                               (r.TestType?.TestName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(urgencyFilter))
                filtered = filtered.Where(r => r.Urgency == urgencyFilter);

            if (!string.IsNullOrWhiteSpace(statusFilter))
                filtered = filtered.Where(r => r.RequestStatus?.StatusName == statusFilter);

            if (!string.IsNullOrWhiteSpace(categoryFilter))
                filtered = filtered.Where(r => r.TestType?.TestCategory?.CategoryName == categoryFilter);

            if (fromDate.HasValue)
                filtered = filtered.Where(r => r.RequestDate >= fromDate.Value);

            if (toDate.HasValue)
                filtered = filtered.Where(r => r.RequestDate <= toDate.Value);

            // Apply sorting
            filtered = (sortOrder?.ToLower() == "desc")
                ? filtered.OrderByDescending(GetSortExpression(sortBy))
                : filtered.OrderBy(GetSortExpression(sortBy));

            var filteredList = filtered.ToList();

            var categories = all.Select(r => r.TestType?.TestCategory?.CategoryName)
                               .Distinct()
                               .Where(c => c != null)
                               .OrderBy(c => c)
                               .ToList();

            var vm = new FilteredTestListViewModel
            {
                Tests = filteredList.ConvertAll(r => new TestListItemViewModel
                {
                    TestRequestId = r.TestRequestId,
                    RequestNumber = r.RequestNumber,
                    PatientName = r.PatientName,
                    TestName = r.TestType?.TestName ?? "",
                    Urgency = r.Urgency,
                    Status = r.RequestStatus?.StatusName ?? "",
                    Category = r.TestType?.TestCategory?.CategoryName ?? "",
                    DueDate = r.DueDate,
                    RequestDate = r.RequestDate,
                    IsOverdue = r.IsOverdue,
                }),
                CurrentFilter = new TableFilterViewModel
                {
                    SearchTerm = searchTerm,
                    UrgencyFilter = urgencyFilter,
                    StatusFilter = statusFilter,
                    CategoryFilter = categoryFilter,
                    FromDate = fromDate,
                    ToDate = toDate,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    AvailableStatuses = StubData.RequestStatuses.ConvertAll(s => s.StatusName),
                    AvailableCategories = categories,
                },
                TotalCount = all.Count,
                FilteredCount = filteredList.Count,
            };

            ViewData["ActivePage"] = "FilteredTests";
            return View(vm);
        }

        private static Func<StubTestRequest, object> GetSortExpression(string? sortBy) => sortBy?.ToLower() switch
        {
            "urgency" => r => r.Urgency,
            "patient" => r => r.PatientName,
            "testname" => r => r.TestType?.TestName ?? "",
            "requestdate" => r => r.RequestDate,
            "duedate" => r => r.DueDate ?? DateTime.MaxValue,
            _ => r => r.DueDate ?? DateTime.MaxValue,  // default to DueDate
        };
    }
}
