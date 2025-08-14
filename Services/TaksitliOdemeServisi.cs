using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Services
{
    public class TaksitliOdemeServisi
    {
        private readonly ApplicationDbContext _veriTabani;

        public TaksitliOdemeServisi(ApplicationDbContext veriTabani)
        {
            _veriTabani = veriTabani;
        }

        // Kullanıcının tüm taksitli ödemelerini getir
        public async Task<List<TaksitliOdeme>> TumTaksitliOdemeleriGetirAsync(int kullaniciId)
        {
            return await _veriTabani.TaksitliOdemeler
                                    .Where(x => x.KullaniciId == kullaniciId)
                                    .ToListAsync();
        }

        // ID ile taksitli ödeme getir
        public async Task<TaksitliOdeme?> IdIleTaksitliOdemeGetirAsync(int id, int kullaniciId)
        {
            return await _veriTabani.TaksitliOdemeler
                                    .FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == kullaniciId);
        }

        // Yeni taksitli ödeme ekle
        public async Task YeniTaksitliOdemeEkleAsync(TaksitliOdeme odeme)
        {
            odeme.BaslangicTarihi = DateTime.Now;
            odeme.KalanTaksit = odeme.TaksitSayisi;
            odeme.BitisTarihi = odeme.BaslangicTarihi.AddMonths(odeme.TaksitSayisi);

            await _veriTabani.TaksitliOdemeler.AddAsync(odeme);
            await _veriTabani.SaveChangesAsync();
        }

        // Taksitli ödemeyi güncelle
        public async Task<bool> TaksitliOdemeyiGuncelleAsync(TaksitliOdeme guncelOdeme, int kullaniciId)
        {
            var eskiOdeme = await IdIleTaksitliOdemeGetirAsync(guncelOdeme.Id, kullaniciId);
            if (eskiOdeme == null) return false;

            eskiOdeme.ToplamTutar = guncelOdeme.ToplamTutar;
            eskiOdeme.TaksitSayisi = guncelOdeme.TaksitSayisi;
            eskiOdeme.FaizOrani = guncelOdeme.FaizOrani;
            eskiOdeme.BaslangicTarihi = guncelOdeme.BaslangicTarihi;
            eskiOdeme.KalanTaksit = guncelOdeme.KalanTaksit;
            eskiOdeme.BitisTarihi = eskiOdeme.BaslangicTarihi.AddMonths(guncelOdeme.TaksitSayisi);

            await _veriTabani.SaveChangesAsync();
            return true;
        }

        // Taksitli ödemeyi sil
        public async Task<bool> TaksitliOdemeyiSilAsync(int id, int kullaniciId)
        {
            var silinecek = await IdIleTaksitliOdemeGetirAsync(id, kullaniciId);
            if (silinecek == null) return false;

            _veriTabani.TaksitliOdemeler.Remove(silinecek);
            await _veriTabani.SaveChangesAsync();
            return true;
        }

        // Taksit ödeme yap
        public async Task<bool> TaksitOdemeYapAsync(int id, int kullaniciId)
        {
            var odeme = await IdIleTaksitliOdemeGetirAsync(id, kullaniciId);
            if (odeme == null || odeme.KalanTaksit <= 0) return false;

            odeme.KalanTaksit--;
            await _veriTabani.SaveChangesAsync();
            return true;
        }
    }
}
