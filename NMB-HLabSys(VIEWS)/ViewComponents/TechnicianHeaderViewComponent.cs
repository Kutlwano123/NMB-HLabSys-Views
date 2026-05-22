// ============================================================
//  TechnicianHeaderViewComponent.cs  —  NMB_HLabSys (VIEWS ONLY)
//  Renders the dynamic topbar profile context block.
//  No Entity Framework Core. No database dependency.
// ============================================================
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NMB_HLabSys.ViewComponents
{
    public class TechnicianHeaderViewComponent : ViewComponent
    {
        // Removed ApplicationDbContext entirely to prevent missing table or connection crashes

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Pull the mock display name injected into the Cookie Claims during the landing page click
            var displayName = HttpContext.User?.Identity?.Name ?? "Guest Technician";

            // Default mock avatar image fallback path
            var photo = "/uploads/default-tech.png";

            // Return the presentation model synchronously to the partial view component layout
            return View(new TechnicianHeaderModel
            {
                Email = displayName,
                PhotoPath = photo
            });
        }
    }

    public class TechnicianHeaderModel
    {
        public string Email { get; set; }
        public string PhotoPath { get; set; }
        public bool HasValidPhoto => !string.IsNullOrWhiteSpace(PhotoPath) && !PhotoPath.Contains("default");
    }
}