using CuzdanUygulamasi.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalWallet.Services.Interfaces
{
    public interface ITaksitliOdemeService
    {
        // Kullanıcıya ait tüm taksitli ödemeleri getir
        Task<List<TaksitliOdeme>> TaksitliOdemeleriGetirAsync(int kullaniciId);

        // Belirli bir ID'ye göre taksitli ödeme getir
        Task<TaksitliOdeme> TaksitliOdemeGetirAsync(int id, int kullaniciId);

        // Yeni bir taksitli ödeme oluştur
        Task<TaksitliOdeme> TaksitliOdemeEkleAsync(TaksitliOdeme odeme);

        // Mevcut bir taksitli ödemeyi güncelle
        Task<TaksitliOdeme> TaksitliOdemeGuncelleAsync(TaksitliOdeme odeme);

        // Belirli bir ID'ye sahip taksitli ödemeyi sil
        Task<bool> TaksitliOdemeSilAsync(int id, int kullaniciId);

        // Kalan tutarı hesapla
        Task<decimal> KalanTutarHesaplaAsync(int id, int kullaniciId);

        // Kalan taksit sayısını hesapla
        Task<int> KalanTaksitSayisiAsync(int id, int kullaniciId);

        // Otomatik taksit ekleme işlemi (ör. ay başında)
        Task<bool> OtomatikTaksitEkleAsync();
    }
}
