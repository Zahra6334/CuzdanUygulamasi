using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Controllers
{
    public class TaksitliOdemeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaksitliOdemeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Taksitlerim sayfası
        public async Task<IActionResult> Index()
        {
            int kullaniciId = GetCurrentUserId();
            var odemeler = await _context.TaksitliOdemeler
                                         .Where(x => x.KullaniciId == kullaniciId)
                                         .ToListAsync();
            return View(odemeler);
        }

        // Yeni taksit ekleme formunu göster
        public IActionResult Create()
        {
            return View();
        }

        // Yeni taksiti kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaksitliOdeme yeniOdeme)
        {
            Console.WriteLine("girdi");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("bunada girdi");
                int kullaniciId = GetCurrentUserId();
                yeniOdeme.KullaniciId = kullaniciId;
                yeniOdeme.BaslangicTarihi = DateTime.Now;
                yeniOdeme.KalanTaksit = yeniOdeme.TaksitSayisi;
                yeniOdeme.BitisTarihi = DateTime.Now.AddMonths(yeniOdeme.TaksitSayisi);

                _context.TaksitliOdemeler.Add(yeniOdeme);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(yeniOdeme);
        }

        // Düzenleme sayfası
        public async Task<IActionResult> Edit(int id)
        {
            int kullaniciId = GetCurrentUserId();
            var odeme = await _context.TaksitliOdemeler
                                      .FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == kullaniciId);
            if (odeme == null) return NotFound();
            return View(odeme);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaksitliOdeme guncellenenOdeme)
        {
            int kullaniciId = GetCurrentUserId();
            if (id != guncellenenOdeme.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var odeme = await _context.TaksitliOdemeler
                                          .FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == kullaniciId);
                if (odeme == null) return NotFound();

                // Güncellenen değerleri ata
                odeme.ToplamTutar = guncellenenOdeme.ToplamTutar;
                odeme.TaksitSayisi = guncellenenOdeme.TaksitSayisi;
                odeme.FaizOrani = guncellenenOdeme.FaizOrani;
                odeme.BaslangicTarihi = guncellenenOdeme.BaslangicTarihi;
                odeme.KalanTaksit = guncellenenOdeme.KalanTaksit;
                odeme.BitisTarihi = odeme.BaslangicTarihi.AddMonths(odeme.TaksitSayisi);

                _context.Update(odeme);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(guncellenenOdeme);
        }

        // Silme sayfası
        public async Task<IActionResult> Delete(int id)
        {
            int kullaniciId = GetCurrentUserId();
            var odeme = await _context.TaksitliOdemeler
                                      .FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == kullaniciId);
            if (odeme == null) return NotFound();
            return View(odeme);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int kullaniciId = GetCurrentUserId();
            var odeme = await _context.TaksitliOdemeler
                                      .FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == kullaniciId);
            if (odeme != null)
            {
                _context.TaksitliOdemeler.Remove(odeme);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Giriş yapan kullanıcı ID'sini al
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new Exception("Kullanıcı giriş yapmamış.");

            return int.Parse(userIdClaim.Value);
        }
    }
}