using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using DinkToPdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;
using Document = QuestPDF.Fluent.Document;
using CuzdanUygulamasi.Extensions;



namespace CuzdanUygulamasi.Controllers
{
    public class OdemeTaksitiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OdemeTaksitiController> _logger;
        public OdemeTaksitiController(ApplicationDbContext context, ILogger<OdemeTaksitiController> logger)
        {
            _context = context;
            _logger= logger;
        }
        // PDF indir
        public IActionResult PdfSayfasi()
        {
            _logger.LogInformation("OdemeTaksitiController -> PdfSayfasi çalıştı.");
            var odemeler = _context.OdemeTaksitleri.ToList();
            return View("OdemePdf", odemeler); // OdemePdf.cshtml'e veri gönderiyoruz
        }

        public async Task<IActionResult> IndirPdf()
        {
            _logger.LogInformation("OdemeTaksitiController -> IndirPdf çalıştı.");
            var odemeler = _context.OdemeTaksitleri.ToList();
            var html = await this.RenderViewAsync("OdemePdf", odemeler, true);

            var converter = new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4
        },
                Objects = {
            new ObjectSettings()
            {
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8" } // Türkçe karakterler için
            }
        }
            };

            var pdf = converter.Convert(doc);
            return File(pdf, "application/pdf", "OdemeTaksitleri.pdf");
        }


        public IActionResult IndirPdfView()
        {
            _logger.LogInformation("OdemeTaksitiController -> IndirPdfView çalıştı.");
            return View(); // IndirPdf.cshtml
        }
        // Ödemeleri listele
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("OdemeTaksitiController -> Index çalıştı.");
            var odemeler = await _context.OdemeTaksitleri
                .Include(o => o.TaksitliOdeme)
                .ToListAsync();
            return View(odemeler);
        }

        // Detay
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var odeme = await _context.OdemeTaksitleri
                .Include(o => o.TaksitliOdeme)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (odeme == null) return NotFound();

            return View(odeme);
        }

        // Yeni ödeme formu
        public IActionResult Create()
        {
            _logger.LogInformation("OdemeTaksitiController -> Create çalıştı.");

            return View();
        }

        // Yeni ödeme ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OdemeTaksiti odeme)
        {
            if (!ModelState.IsValid)
            {
                
                odeme.OdemeTarihi = DateTime.Now; // otomatik tarih
                _context.OdemeTaksitleri.Add(odeme);
                
                // Taksitli ödeme tablosunu güncelle
                var taksitliOdeme = await _context.TaksitliOdemeler.FindAsync(odeme.TaksitliOdemeId);
                if (taksitliOdeme != null && taksitliOdeme.KalanTaksit > 0)
                {
                    taksitliOdeme.KalanTaksit--;
                    _context.TaksitliOdemeler.Update(taksitliOdeme);
                }
                var bildirim = new Bildirim
                {
                    KullaniciId = taksitliOdeme.KullaniciId, // ilgili kullanıcı
                    Mesaj = $"Yeni ödeme yapıldı: {odeme.OdenenTutar:N2}₺",
                    Tarih = DateTime.Now,
                    OkunduMu = false
                };
                _context.Bildirimler.Add(bildirim);
                await _context.SaveChangesAsync(); // 🔑 DB'ye yazma noktası
                return RedirectToAction(nameof(Index));
            }
            return View(odeme);
        }

        // Düzenle (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var odeme = await _context.OdemeTaksitleri.FindAsync(id);
            if (odeme == null) return NotFound();

            return View(odeme);
        }

        // Düzenle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OdemeTaksiti odeme)
        {
            if (id != odeme.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(odeme);
                    await _context.SaveChangesAsync(); // 🔑 DB'ye güncelleme
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.OdemeTaksitleri.Any(e => e.Id == odeme.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(odeme);
        }

        // Sil (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var odeme = await _context.OdemeTaksitleri
                .Include(o => o.TaksitliOdeme)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (odeme == null) return NotFound();

            return View(odeme);
        }

        // Sil (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var odeme = await _context.OdemeTaksitleri.FindAsync(id);
            if (odeme != null)
            {
                _context.OdemeTaksitleri.Remove(odeme);
                await _context.SaveChangesAsync(); // 🔑 DB'den silme
            }
            return RedirectToAction(nameof(Index));
        }

        // Senin özel methodun: Ödeme yap
        [HttpPost]
        public async Task<IActionResult> OdemeYap(int taksitliOdemeId, decimal odenenTutar, string odemeYontemi)
        {

            var taksitliOdeme = await _context.TaksitliOdemeler.FindAsync(taksitliOdemeId);
            if (taksitliOdeme == null)
                return NotFound();

            var odeme = new OdemeTaksiti
            {
                TaksitliOdemeId = taksitliOdemeId,
                OdenenTutar = odenenTutar,
                OdemeYontemi = odemeYontemi,
                OdemeTarihi = DateTime.Now
            };

            _context.OdemeTaksitleri.Add(odeme);

            if (taksitliOdeme.KalanTaksit > 0)
                taksitliOdeme.KalanTaksit--;

            _context.Update(taksitliOdeme);
            await _context.SaveChangesAsync(); // 🔑 DB’ye kayıt noktası

            return RedirectToAction("Detay", "TaksitliOdemeMvc", new { id = taksitliOdemeId });
        }
    }
}
