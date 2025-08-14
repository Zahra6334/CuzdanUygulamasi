using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // Silme işlemi
        [HttpPost]
        public async Task<IActionResult> IslemSil(int id)
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
    }
}
