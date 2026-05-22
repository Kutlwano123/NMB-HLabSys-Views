// ============================================================
//  Program.cs  —  NMB_HLabSys (VIEWS)
//  Minimal setup: cookie auth only. No EF Core. No Identity.
// ============================================================
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ── MVC ────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── Session (for storing technician settings) ──────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Cookie authentication (replaces Identity entirely) ─────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/";          // unauthenticated → back to landing page
        options.AccessDeniedPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ── NO DbContext, NO Identity, NO LabWorkflowService ───────
// Everything runs from NMB_HLabSys_VIEWS.Data.StubData

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();           // must come before UseAuthentication
app.UseAuthentication();   // must come before UseAuthorization
app.UseAuthorization();

// Default route → HomeController.Index (landing page)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();