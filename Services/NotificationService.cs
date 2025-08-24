using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Kullanıcıya bildirim ekleme
        public async Task BildirimGonderAsync(int kullaniciId, string mesaj)
        {
            var bildirim = new Bildirim
            {
                KullaniciId = kullaniciId,
                Mesaj = mesaj,
                Tarih = DateTime.Now,
                OkunduMu = false
            };

            await _context.Bildirimler.AddAsync(bildirim);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"📩 Bildirim eklendi -> KullanıcıId: {kullaniciId}, Mesaj: {mesaj}");
        }

        // Ödenmemiş taksitleri kontrol et ve bildirim oluştur
        public async Task KontrolEtVeBildirimOlusturAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var taksitler = await _context.OdemeTaksitleri
                .Include(t => t.Islem)
                .Where(t => !t.OdendiMi &&
                            (t.SonOdemeTarihi.Date == today || t.SonOdemeTarihi.Date == tomorrow))
                .ToListAsync();

            var bildirimler = new List<Bildirim>();

            foreach (var taksit in taksitler)
            {
                if (taksit.Islem == null) continue;

                var userId = taksit.Islem.KullaniciId;

                string mesaj = taksit.SonOdemeTarihi.Date == today
                    ? $"Bugün ödenmesi gereken taksit: {taksit.TaksitliOdeme:N2}₺ ({taksit.Islem.Aciklama})"
                    : $"Yarın ödenmesi gereken taksit: {taksit.TaksitliOdeme:N2}₺ ({taksit.Islem.Aciklama})";

                bildirimler.Add(new Bildirim
                {
                    KullaniciId = userId,
                    Mesaj = mesaj,
                    Tarih = DateTime.Now,
                    OkunduMu = false
                });
            }

            if (bildirimler.Any())
            {
                await _context.Bildirimler.AddRangeAsync(bildirimler);
                await _context.SaveChangesAsync();
            }
        }

    }
}

