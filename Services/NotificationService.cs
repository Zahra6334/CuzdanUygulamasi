using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kullanıcıya bildirim ekleme
        public void BildirimGonder(int kullaniciId, string mesaj)
        {
            var bildirim = new Bildirim
            {
                KullaniciId = kullaniciId,
                Mesaj = mesaj,
                Tarih = DateTime.Now,
                OkunduMu = false
            };

            _context.Bildirimler.Add(bildirim);
            _context.SaveChanges();
        }

        // Ödenmemiş taksitleri kontrol et ve bildirim oluştur
        public void KontrolEtVeBildirimOlustur()
        {
            var today = DateTime.Today;

            var taksitler = _context.OdemeTaksitleri
                .Include("Islem")
                .Where(t => !t.OdendiMi &&
                            (t.SonOdemeTarihi.Date == today || t.SonOdemeTarihi.Date == today.AddDays(1)))
                .ToList();

            foreach (var taksit in taksitler)
            {
                var userId = taksit.Islem.KullaniciId;

                string mesaj = taksit.SonOdemeTarihi.Date == today
                    ? $"Bugün ödenmesi gereken taksit: {taksit.OdenenTutar:N2}₺ ({taksit.Islem.Aciklama})"
                    : $"Yarın ödenmesi gereken taksit: {taksit.OdenenTutar:N2}₺ ({taksit.Islem.Aciklama})";

                BildirimGonder(userId, mesaj);
            }
        }

        internal async Task KontrolEtVeBildirimOlusturAsync()
        {
            throw new NotImplementedException();
        }
    }
}
