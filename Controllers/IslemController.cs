using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using CuzdanUygulamasi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Controllers
{
    public class IslemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IslemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index: Sadece giriş yapan kullanıcıya ait işlemler
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return View(new List<Islem>());

            int userId = int.Parse(userIdStr);

            var islemler = await _context.Islemler
                .Where(i => i.KullaniciId == userId)
                .Include(i => i.Kategori)
                .Include(i => i.Kullanici)
                .OrderByDescending(i => i.Tarih)
                .ToListAsync();

            return View(islemler);
        }

        // GET Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Kategoriler = _context.Kategoriler.ToList();
            return View();
        }

        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Islem islem)
        {
            ViewBag.Kategoriler = _context.Kategoriler.ToList();

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                ModelState.AddModelError("", "Kullanıcı login değil.");
                return View(islem);
            }

            islem.KullaniciId = int.Parse(userIdStr);
            islem.Tarih = System.DateTime.Now;

            _context.Islemler.Add(islem);
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return RedirectToAction("Index");
                else
                    ModelState.AddModelError("", "Kayıt eklenemedi.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Hata: " + ex.Message);
            }

            return View(islem);
        }

        // Edit (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Index");

            int userId = int.Parse(userIdStr);

            var islem = await _context.Islemler
                .FirstOrDefaultAsync(i => i.Id == id && i.KullaniciId == userId);

            if (islem == null)
                return NotFound();

            ViewBag.Kategoriler = _context.Kategoriler.ToList();
            return View(islem);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Islem islem)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Index");

            int userId = int.Parse(userIdStr);

            if (islem.KullaniciId != userId)
                return Unauthorized();

            if (ModelState.IsValid)
            {
                ViewBag.Kategoriler = _context.Kategoriler.ToList();
                return View(islem);
            }

            // Mevcut veriyi al ve gerekli alanları koru
            var mevcutIslem = await _context.Islemler.AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == islem.Id && i.KullaniciId == userId);

            if (mevcutIslem == null)
                return NotFound();

            islem.Tarih = DateTime.Now; // güncelleme sonrası liste başında çıkar
                                     
            islem.KullaniciId = mevcutIslem.KullaniciId;

            _context.Update(islem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Delete (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Index");

            int userId = int.Parse(userIdStr);

            var islem = await _context.Islemler
                .Include(i => i.Kategori)
                .FirstOrDefaultAsync(i => i.Id == id && i.KullaniciId == userId);

            if (islem == null)
                return NotFound();

            return View(islem);
        }

        // DeleteConfirmed (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Index");

            int userId = int.Parse(userIdStr);

            var silinecek = await _context.Islemler
                .FirstOrDefaultAsync(i => i.Id == id && i.KullaniciId == userId);

            if (silinecek != null)
            {
                _context.Islemler.Remove(silinecek);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // IslemController'a bu action'ı ekleyin
        public async Task<IActionResult> Analiz()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Index");

            int userId = int.Parse(userIdStr);

            var model = new AnalizViewModel();

            // Aylık Gelir-Gider Verileri
            model.AylikGelirGider = await _context.Islemler
                .Where(i => i.KullaniciId == userId)
                .GroupBy(i => new { i.Tarih.Year, i.Tarih.Month, i.IslemTipi })
                .Select(g => new AylikGelirGider
                {
                    Yil = g.Key.Year,
                    Ay = g.Key.Month,
                    IslemTipi = g.Key.IslemTipi,
                    Toplam = g.Sum(i => i.Tutar)
                })
                .ToListAsync();

            // Kategori Dağılımı
            model.KategoriDagilimi = await _context.Islemler
                .Where(i => i.KullaniciId == userId && i.IslemTipi == IslemTipi.Gider)
                .Include(i => i.Kategori)
                .GroupBy(i => i.Kategori.Ad)
                .Select(g => new KategoriDagilimi
                {
                    KategoriAdi = g.Key,
                    Toplam = g.Sum(i => i.Tutar)
                })
                .ToListAsync();

            // Taksit Özeti (Eğer taksit özelliğiniz varsa)
            model.TaksitOzeti = await _context.Islemler
                .Where(i => i.KullaniciId == userId )
                .GroupBy(i => i.TekrarliMi)
                .Select(g => new TaksitOzeti
                {
                    
                    ToplamTutar = g.Sum(i => i.Tutar)
                })
                .ToListAsync();

            return View(model);
        }


    }
}
