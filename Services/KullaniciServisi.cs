using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;
using CuzdanUygulamasi.Services.Arayuzler;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Services
{
    public class KullaniciServisi : IKullaniciServisi
    {
        private readonly ApplicationDbContext _veriTabani;

        public KullaniciServisi(ApplicationDbContext veriTabani)
        {
            _veriTabani = veriTabani;
        }

        public async Task<List<Kullanici>> TumKullanicilariGetirAsync()
        {
            return await _veriTabani.Kullanicilar.ToListAsync();
        }

        public async Task<Kullanici> KullaniciGetirAsync(int id)
        {
            return await _veriTabani.Kullanicilar.FindAsync(id);
        }

        public async Task<Kullanici> KullaniciOlusturAsync(Kullanici kullanici)
        {
            _veriTabani.Kullanicilar.Add(kullanici);
            await _veriTabani.SaveChangesAsync();
            return kullanici;
        }

        public async Task<bool> KullaniciGuncelleAsync(Kullanici kullanici)
        {
            _veriTabani.Kullanicilar.Update(kullanici);
            return await _veriTabani.SaveChangesAsync() > 0;
        }

        public async Task<bool> KullaniciSilAsync(int id)
        {
            var kullanici = await _veriTabani.Kullanicilar.FindAsync(id);
            if (kullanici == null)
                return false;

            _veriTabani.Kullanicilar.Remove(kullanici);
            return await _veriTabani.SaveChangesAsync() > 0;
        }
    }
}
