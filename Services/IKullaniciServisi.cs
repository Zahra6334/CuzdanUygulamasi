using CuzdanUygulamasi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Services.Arayuzler
{
    public interface IKullaniciServisi
    {
        Task<List<Kullanici>> TumKullanicilariGetirAsync();
        Task<Kullanici> KullaniciGetirAsync(int id);
        Task<Kullanici> KullaniciOlusturAsync(Kullanici kullanici);
        Task<bool> KullaniciGuncelleAsync(Kullanici kullanici);
        Task<bool> KullaniciSilAsync(int id);
    }
}
