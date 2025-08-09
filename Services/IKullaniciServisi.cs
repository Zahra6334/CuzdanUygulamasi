using CuzdanUygulamasi.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Services
{
    public interface IKullaniciServisi
    {
        Task<List<Kullanici>> TumKullanicilariGetirAsync();
        Task<Kullanici> KullaniciGetirAsync(int id);
        Task<Kullanici> KullaniciOlusturAsync(Kullanici kullanici);
        Task<bool> KullaniciGuncelleAsync(Kullanici kullanici);
        Task<bool> KullaniciSilAsync(int id);
        Task Authenticate(string kullaniciAdi, string sifre);
      
        Task<bool> EmailVarMi(string email);
        Task<Kullanici> FindByEmailAndPasswordAsync(string email, string sifre);

        // ✨ Şifre sıfırlama için gerekli olanlar:
        Task<Kullanici?> FindByEmailAsync(string email);
        Task<Kullanici?> FindByIdAsync(string id); // Id'nin tipi string ise
        Task<string> GeneratePasswordResetTokenAsync(Kullanici user);
        Task<IdentityResult> ResetPasswordAsync(Kullanici user, string token, string newPassword);
    
}
}
