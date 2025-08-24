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
            _logger.LogInformation("HomeController -> Index çalıştı.");
            
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("HomeController -> Privacy çalıştı.");
            return View();
        }
        public async Task<IActionResult> Currency()
        {
            _logger.LogInformation("HomeController -> Currency çalıştı.");
            var result = await _exchangeRateService.GetRatesAsync("TRY", "USD,EUR,GBP");
            return View(result); // View'e result modelini gönderiyoruz
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogInformation("HomeController -> Error çalıştı.");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        

        // Ödenmemiş taksitleri kontrol edip bildirim oluştur
        public async Task<IActionResult> CheckTaksitNotifications()
        {
            _logger.LogInformation("HomeController -> Bildirim kontrolü çalıştı.");
            await _notificationService.KontrolEtVeBildirimOlusturAsync();
            return RedirectToAction("Index");
        }

        // Kullanıcıya bildirimleri göster
        public ActionResult Bildirimler()
        {
            if (!(User.Identity is ClaimsIdentity identity))
            {
                _logger.LogWarning("Bildirimler -> Kullanıcı giriş yapmamış!");
                return RedirectToAction("Index");
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Bildirimler -> Kullanıcı ID bulunamadı!");
                return RedirectToAction("Index");
            }

            int userId = int.Parse(userIdClaim.Value);

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

            _logger.LogInformation($"Bildirimler -> KullanıcıId={userId}, Bildirim sayısı={bildirimler.Count}");

            return View(bildirimler);
        }
    }
    
    
}
