using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using Microsoft.EntityFrameworkCore;

namespace CuzdanUygulamasi.Services
{
    public class IslemServisi
    {
        private readonly ApplicationDbContext _context;

        public IslemServisi(ApplicationDbContext context)
        {
            _context = context;
        }

        public void IslemEkle(Islem islem)
        {
            _context.Islemler.Add(islem);
            _context.SaveChanges();
        }

        public decimal ToplamGelirHesapla(int kullaniciId)
        {
            return _context.Islemler
                .Where(i => i.KullaniciId == kullaniciId && i.IslemTipi == IslemTipi.Gelir)
                .Sum(i => (decimal?)i.Tutar) ?? 0m; // null check
        }

        public decimal ToplamGiderHesapla(int kullaniciId)
        {
            return _context.Islemler
                .Where(i => i.KullaniciId == kullaniciId && i.IslemTipi == IslemTipi.Gider)
                .Sum(i => (decimal?)i.Tutar) ?? 0m; // null check
        }

        public decimal BakiyeHesapla(int kullaniciId)
        {
            return ToplamGelirHesapla(kullaniciId) - ToplamGiderHesapla(kullaniciId);
        }
    }
}
