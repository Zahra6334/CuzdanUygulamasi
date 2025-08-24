using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Helpers;
using CuzdanUygulamasi.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Serilog Baþlangýç ------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // appsettings.json'dan oku
    .CreateLogger();

builder.Host.UseSerilog();
// ------------------ Serilog Bitiþ ------------------

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IKullaniciServisi, KullaniciServisi>();
builder.Services.AddScoped<TaksitliOdemeServisi>();
builder.Services.AddScoped<ExchangeRateService>();
builder.Services.AddScoped<NotificationService>();

var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox", "libwkhtmltox.dll"));

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Login sayfasý
        options.LogoutPath = "/Account/Logout"; // Logout sayfasý
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Cookie süresi
    });

var app = builder.Build();

// ------------------ Serilog Request Logging ------------------
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

// CorrelationId + UserId enrich
app.Use(async (ctx, next) =>
{
    var cid = ctx.Request.Headers.ContainsKey("X-Correlation-ID")
        ? ctx.Request.Headers["X-Correlation-ID"].ToString()
        : Activity.Current?.Id ?? Guid.NewGuid().ToString();

    var userId = ctx.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    using (LogContext.PushProperty("CorrelationId", cid))
    using (LogContext.PushProperty("UserId", userId ?? "anonymous"))
    {
        ctx.Response.Headers["X-Correlation-ID"] = cid;
        await next();
    }
});
// ------------------ Serilog Request Logging Bitiþ ------------------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
