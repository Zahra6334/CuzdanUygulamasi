using CuzdanUygulamasi.Data;
using CuzdanUygulamasi.Models;

using Microsoft.AspNetCore.Identity;
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

        public Task Authenticate(string kullaniciAdi, string sifre)
        {
            throw new NotImplementedException();
        }

        public Task<Kullanici?> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Kullanici?> FindByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GeneratePasswordResetTokenAsync(Kullanici user)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ResetPasswordAsync(Kullanici user, string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> EmailVarMi(string email)
        {
            return await _veriTabani.Kullanicilar.AnyAsync(u => u.Email == email);
        }

        public async Task<Kullanici> FindByEmailAndPasswordAsync(string email, string sifre)
        {
            return await _veriTabani.Kullanicilar
                .FirstOrDefaultAsync(u => u.Email == email && u.SifreHash == sifre);
        }
    }
}
