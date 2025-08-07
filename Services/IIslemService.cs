using CuzdanUygulamasi.Models;

using System.Collections.Generic;

namespace PersonalWallet.Services
{
    public interface IIslemService
    {
        List<Islem> TumIslemleriGetir();
        Islem IdIleGetir(int id);
        void Ekle(Islem islem);
        void Guncelle(Islem islem);
        void Sil(int id);
        List<Islem> KullanicininIslemleri(int kullaniciId);
    }
}
