using CuzdanUygulamasi.Models.ViewModels;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CuzdanUygulamasi.Services;

namespace CuzdanUygulamasi.Controllers
{

    public class HomeController : Controller
    {
        private readonly ExchangeRateService _exchangeRateService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Currency()
        {
            var result = await _exchangeRateService.GetRatesAsync("TRY", "USD,EUR,GBP");
            return View(result); // View'e result modelini gönderiyoruz
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
