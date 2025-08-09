using CuzdanUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CuzdanUygulamasi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KullaniciController : ControllerBase
    {
        private static List<Kullanici> kullanicilar = new List<Kullanici>();

        [HttpGet]
        public IActionResult TumKullanicilariGetir()
        {
            return Ok(kullanicilar);
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
