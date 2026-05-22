// ============================================================
//  HomeController.cs  —  NMB_HLabSys (VIEWS ONLY)
//  Serves the landing page and handles zero-login role bypass routing.
// ============================================================
using Microsoft.AspNetCore.Mvc;
using NMB_HLabSys_VIEWS_.Models;
using System.Diagnostics;

namespace NMB_HLabSys_VIEWS_.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Demo login — triggers instantly when clicking any role portal card
        [HttpGet]
        [Route("Account/DemoLogin")]
        public IActionResult DemoLogin(string role = "LabTechnician")
        {
            // Assign custom display variables to mimic an active system database profile
            var (userId, userName) = role switch
            {
                "Admin" => ("demo-admin", "Kutlwano Modise"),
                "Doctor" => ("demo-doctor", "Dr. A. Petersen"),
                "LabTechnician" => ("demo-tech", "Kabelo Mabidikama"),
                "LabManager" => ("demo-manager", "Unathi Mzathu"),
                "Patient" => ("demo-patient", "Pieter Selekane"),
                _ => ("demo-tech", "Kabelo Mabidikama"),
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name,           userName),
                new Claim(ClaimTypes.Role,           role),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Establish the fake authentication session securely
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

            // Route user directly to their mockup layouts
            return role switch
            {
                "LabTechnician" => RedirectToAction("Dashboard", "LabTechnician"),
                "LabManager" => RedirectToAction("Index", "LabManager"),
                "Doctor" => RedirectToAction("Index", "Doctor"),
                "Admin" => RedirectToAction("Index", "Admin"),
                "Patient" => RedirectToAction("Index", "Patient"),
                _ => RedirectToAction("Dashboard", "LabTechnician"),
            };
        }

        [HttpGet]
        [Route("Account/Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction(nameof(Index));
        }
    }
}