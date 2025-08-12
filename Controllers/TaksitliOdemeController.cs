using CuzdanUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CuzdanUygulamasi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaksitliOdemeController : ControllerBase
    {
        private static List<TaksitliOdeme> taksitliOdemeler = new List<TaksitliOdeme>();

        [HttpGet]
        public IActionResult TumTaksitleriGetir()
        {
            return Ok(taksitliOdemeler);
        }

        [HttpPost]
        public IActionResult TaksitEkle([FromBody] TaksitliOdeme yeniOdeme)
        {
            taksitliOdemeler.Add(yeniOdeme);
            return Ok("Taksitli ödeme eklendi.");
        }

        [HttpPut("{id}")]
        public IActionResult TaksitGuncelle(int id, [FromBody] TaksitliOdeme guncellenenOdeme)
        {
            var mevcut = taksitliOdemeler.FirstOrDefault(t => t.Id == id);
            if (mevcut == null) return NotFound("Taksitli ödeme bulunamadı.");

            mevcut.ToplamTutar = guncellenenOdeme.ToplamTutar;
            mevcut.TaksitSayisi = guncellenenOdeme.TaksitSayisi;
            mevcut.BaslangicTarihi = guncellenenOdeme.BaslangicTarihi;
            mevcut.KullaniciId = guncellenenOdeme.KullaniciId;

            return Ok("Taksitli ödeme güncellendi.");
        }

        [HttpDelete("{id}")]
        public IActionResult TaksitSil(int id)
        {
            var silinecek = taksitliOdemeler.FirstOrDefault(t => t.Id == id);
            if (silinecek == null) return NotFound("Taksitli ödeme bulunamadı.");

            taksitliOdemeler.Remove(silinecek);
            return Ok("Taksitli ödeme silindi.");
        }
    }
}
