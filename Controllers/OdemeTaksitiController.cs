using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;
using Document = QuestPDF.Fluent.Document;



namespace CuzdanUygulamasi.Controllers
{
    public class OdemeTaksitiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OdemeTaksitiController(ApplicationDbContext context)
        {
            _context = context;
        }
        // PDF indir
        public IActionResult IndirPdf()
        {
            var odemeler = _context.OdemeTaksitleri.ToList();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header
                    page.Header()
                        .Text("Ödeme Taksitleri Listesi")
                        .SemiBold().FontSize(16).FontColor(Colors.Black)
                        .AlignCenter();

                    // Tek Content çağrısı
                    page.Content()
                        .Table(table =>
                        {
                            // 4 kolon
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Başlık satırı
                            table.Header(header =>
                            {
                                header.Cell().Text("Id");
                                header.Cell().Text("Ödenen Tutar");
                                header.Cell().Text("Ödeme Tarihi");
                                header.Cell().Text("Ödeme Yöntemi");
                            });

                            // Satırlar
                            foreach (var item in odemeler)
                            {
                                table.Cell().Text(item.Id.ToString());
                                table.Cell().Text(item.OdenenTutar.ToString());
                                table.Cell().Text(item.OdemeTarihi.ToShortDateString());
                                table.Cell().Text(item.OdemeYontemi ?? "-");
                            }
                        });

                    // Footer
                    page.Footer()
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("Sayfa ");
                            txt.CurrentPageNumber();
                            txt.Span(" / ");
                            txt.TotalPages();
                        });
                });
            });

            var pdfBytes = pdf.GeneratePdf();

            return File(pdfBytes, "application/pdf", "OdemeTaksitleri.pdf");
        }
        public IActionResult IndirPdfView()
        {
            return View(); // IndirPdf.cshtml
        }
        // Ödemeleri listele
        public async Task<IActionResult> Index()
        {
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
