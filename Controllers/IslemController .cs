using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CuzdanUygulamasi.Controllers
{
    public class IslemController : Controller
    {
        private static List<Islem> islemler = new List<Islem>();
        private readonly ApplicationDbContext _context;
        public IslemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ana sayfa (tüm işlemler)
        public IActionResult Index()
        {
            return View(islemler);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Create(Islem islem)
        {
            if (ModelState.IsValid)
            {
                _context.Islemler.Add(islem);
                _context.SaveChanges();
                return RedirectToAction("Create"); // işlem listesine geri dön
            }
            return View(islem); // hatalıysa form tekrar gösterilir
        }


        // İşlem ekleme (form ile)
        [HttpPost]
        public IActionResult IslemEkle(Islem yeniIslem)
        {
            if (yeniIslem != null)
            {
                yeniIslem.Id = islemler.Count > 0 ? islemler.Max(i => i.Id) + 1 : 1;
                yeniIslem.Tarih = DateTime.Now;
                islemler.Add(yeniIslem);
            }
            return RedirectToAction("Index");
        }
        // İşlem silme
        [HttpPost]
        public IActionResult IslemSil(int id)
        {
            var silinecek = islemler.FirstOrDefault(i => i.Id == id);
            if (silinecek != null)
                islemler.Remove(silinecek);

            return RedirectToAction("Index");
        }
    }
}
