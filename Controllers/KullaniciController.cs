using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using CuzdanUygulamasi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CuzdanUygulamasi.Services;

namespace CuzdanUygulamasi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class KullaniciController : Controller
    {
        // Test amaçlı hafızada kullanıcı listesi (normalde DB'den çekilir)
        private static List<Kullanici> kullanicilar = new List<Kullanici>();
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ExchangeRateService _exchangeRateService;

        public KullaniciController(ApplicationDbContext context, IWebHostEnvironment env,ExchangeRateService exchangeRateService)
        {

            _context = context;
            _env = env;
            _exchangeRateService = exchangeRateService;
        }
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null || int.Parse(userIdStr) != id)
                return Unauthorized(); // Başkasının profilini görmesin

            var kullanici = _context.Kullanicilar
                .Include(k => k.Islemler)  // Kullanıcının işlemlerini çek
                .ThenInclude(i => i.Kategori)
                .Include(k => k.Islemler)
                .ThenInclude(i => i.TaksitliOdeme)
                .FirstOrDefault(k => k.Id == id);

            if (kullanici == null)
                return NotFound();
            var result = await _exchangeRateService.GetRatesAsync("TRY", "USD,EUR,GBP");
            var vm = new ProfilViewModel
            {
                KullaniciId = kullanici.Id,
                KullaniciAdi = kullanici.AdSoyad,
                Email = kullanici.Email,
                ProfilPic=kullanici.ProfiPic,
                ToplamGelir = kullanici.Islemler
                                .Where(i => i.IslemTipi == IslemTipi.Gelir)
                                .Sum(i => i.Tutar),
                ToplamGider = kullanici.Islemler
                                .Where(i => i.IslemTipi == IslemTipi.Gider)
                                .Sum(i => i.Tutar),
                IslemSayisi = kullanici.Islemler.Count,
                TaksitSayisi = kullanici.Islemler.Count(i => i.IslemTipi == IslemTipi.Taksit),
                KategoriSayisi = kullanici.Islemler.Where(i => i.KategoriId != null).Select(i => i.KategoriId).Distinct().Count(),
                SonIslemler = kullanici.Islemler
                                .OrderByDescending(i => i.Tarih)
                                .Take(5)
                                .ToList(),
                DovizKurlari = new Dictionary<string, decimal>
        {
            { "USD", 1 / result.Rates["USD"] }, // 1 USD = ? TL
            { "EUR", 1 / result.Rates["EUR"] }, // 1 EUR = ? TL
            { "GBP", 1 / result.Rates["GBP"] }  // 1 GBP = ? TL
        }
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> ProfilResmiDegistir(IFormFile profilResmi)
        {
            var kullaniciId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (profilResmi != null && profilResmi.Length > 0)
            {
               
                var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);

                if (kullanici != null)
                {
                    // Klasör yolu
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Dosya adı
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilResmi.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    // Dosyayı kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilResmi.CopyToAsync(stream);
                    }

                    // Kullanıcının profil fotoğrafını güncelle
                    kullanici.ProfiPic = "/uploads/" + fileName;
                    _context.Update(kullanici);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Details", "Kullanici", new { id = kullaniciId });

        }

       

        [HttpPost]
        public IActionResult KullaniciEkle([FromBody] Kullanici yeniKullanici)
        {
            kullanicilar.Add(yeniKullanici);
            return Ok("Kullanıcı eklendi.");
        }

        [HttpPut("{id}")]
        public IActionResult KullaniciGuncelle(int id, [FromBody] Kullanici guncellenenKullanici)
        {
            var mevcut = kullanicilar.FirstOrDefault(k => k.Id == id);
            if (mevcut == null) return NotFound("Kullanıcı bulunamadı.");

            mevcut.AdSoyad = guncellenenKullanici.AdSoyad;
            mevcut.Email = guncellenenKullanici.Email;
            mevcut.SifreHash = guncellenenKullanici.SifreHash;
            mevcut.KayitTarihi = guncellenenKullanici.KayitTarihi;

            return Ok("Kullanıcı güncellendi.");
        }
        [HttpPost("ProfilFotoGuncelle")]
        public async Task<IActionResult> ProfilFotoGuncelle(int id, IFormFile profilFoto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null || int.Parse(userIdStr) != id)
                return Unauthorized();

            if (profilFoto != null && profilFoto.Length > 0)
            {
                // Kayıt yolu
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Benzersiz dosya adı
                var uniqueFileName = $"{Guid.NewGuid()}_{profilFoto.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilFoto.CopyToAsync(stream);
                }

                // DB'de güncelle
                var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Id == id);
                if (kullanici == null)
                    return NotFound();

                kullanici.ProfiPic= $"/images/profiles/{uniqueFileName}";
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = id });
            }

            return BadRequest("Geçerli bir fotoğraf seçilmedi.");
        }


        [HttpDelete("{id}")]
        public IActionResult KullaniciSil(int id)
        {
            var silinecek = kullanicilar.FirstOrDefault(k => k.Id == id);
            if (silinecek == null) return NotFound("Kullanıcı bulunamadı.");

            kullanicilar.Remove(silinecek);
            return Ok("Kullanıcı silindi.");
        }
    }
}
