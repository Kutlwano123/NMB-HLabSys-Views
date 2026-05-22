// ============================================================
//  TechnicianHeaderViewComponent.cs  —  NMB_HLabSys (VIEWS ONLY)
//  Renders the dynamic topbar profile context block.
//  No Entity Framework Core. No database dependency.
// ============================================================
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using NMB_HLabSys_VIEWS.Data;

namespace NMB_HLabSys.ViewComponents
{
    public class TechnicianHeaderViewComponent : ViewComponent
    {
        // Removed ApplicationDbContext entirely to prevent missing table or connection crashes

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Pull the mock display name injected into the Cookie Claims during the landing page click
            var displayName = HttpContext.User?.Identity?.Name ?? "Guest Technician";
            var userId = HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "demo-tech";

            // Default mock avatar image fallback path
            var photo = "/uploads/default-tech.png";

            // Generate notifications for this technician
            StubData.GenerateNotificationsForTechnician(userId);
            var unreadCount = StubData.UnreadNotificationsForTechnician(userId).Count;

            // Return the presentation model synchronously to the partial view component layout
            return View(new TechnicianHeaderModel
            {
                Email = displayName,
                PhotoPath = photo,
                UnreadNotificationCount = unreadCount,
            });
        }
    }

    public class TechnicianHeaderModel
    {
        public string Email { get; set; }
        public string PhotoPath { get; set; }
        public int UnreadNotificationCount { get; set; }
        public bool HasValidPhoto => !string.IsNullOrWhiteSpace(PhotoPath) && !PhotoPath.Contains("default");
    }
}