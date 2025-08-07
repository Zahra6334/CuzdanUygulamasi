using CuzdanUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;
using CuzdanUygulamasi.Models;

namespace CuzdanUygulamasi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IslemController : ControllerBase
    {
        private static List<Islem> islemler = new List<Islem>();

        [HttpGet]
        public IActionResult TumIslemleriGetir()
        {
            return Ok(islemler);
        }

        [HttpPost]
        public IActionResult IslemEkle([FromBody] Islem yeniIslem)
        {
            islemler.Add(yeniIslem);
            return Ok("İşlem eklendi.");
        }

        [HttpPut("{id}")]
        public IActionResult IslemGuncelle(int id, [FromBody] Islem guncellenenIslem)
        {
            var mevcut = islemler.FirstOrDefault(i => i.Id == id);
            if (mevcut == null) return NotFound("İşlem bulunamadı.");

            mevcut.IslemTipi = guncellenenIslem.IslemTipi;
            mevcut.Tutar = guncellenenIslem.Tutar;
            mevcut.Tarih = guncellenenIslem.Tarih;
            mevcut.Aciklama = guncellenenIslem.Aciklama;
            mevcut.KullaniciId = guncellenenIslem.KullaniciId;

            return Ok("İşlem güncellendi.");
        }

        [HttpDelete("{id}")]
        public IActionResult IslemSil(int id)
        {
            var silinecek = islemler.FirstOrDefault(i => i.Id == id);
            if (silinecek == null) return NotFound("İşlem bulunamadı.");

            islemler.Remove(silinecek);
            return Ok("İşlem silindi.");
        }
    }
}
