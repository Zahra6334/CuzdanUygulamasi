using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models.ViewModels;
using CuzdanUygulamasi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Linq;

namespace CuzdanUygulamasi.Controllers
{

    public class HomeController : Controller
    {
        private readonly ExchangeRateService _exchangeRateService;
        private readonly NotificationService _notificationService;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, NotificationService notificationService, ExchangeRateService exchangeRateService, ApplicationDbContext context)
        {
            _exchangeRateService = exchangeRateService;
            _notificationService = notificationService;
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            _notificationService.KontrolEtVeBildirimOlustur();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Currency()
        {
            var result = await _exchangeRateService.GetRatesAsync("TRY", "USD,EUR,GBP");
            return View(result); // View'e result modelini g�nderiyoruz
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // Kullan�c� giri� yapt���nda veya sayfa a��ld���nda �a�r�labilir
        
        public async Task<IActionResult> CheckTaksitNotifications()
        {
            await _notificationService.KontrolEtVeBildirimOlusturAsync();
            return RedirectToAction("Index");
        }


        // Kullan�c�ya bildirimleri g�ster
        public ActionResult Bildirimler()
        {
            var userIdClaim = User.Identity as ClaimsIdentity;
            var userId = int.Parse(userIdClaim.FindFirst(ClaimTypes.NameIdentifier).Value);

            var bildirimler = _context.Bildirimler
                .Where(b => b.KullaniciId == userId && !b.OkunduMu)
                .OrderByDescending(b => b.Tarih)
                .Select(b => new BildirimViewModel
                {
                    Id = b.Id,
                    Mesaj = b.Mesaj,
                    Tarih = b.Tarih,
                    OkunduMu = b.OkunduMu
                })
                .ToList();

            return View(bildirimler);
        }

        // �denmemi� taksitleri kontrol edip bildirim olu�tur
        public async Task<IActionResult> Notifications()
        {
            await _notificationService.KontrolEtVeBildirimOlusturAsync();
            return RedirectToAction("Index");
        }

    }
    
    
}
