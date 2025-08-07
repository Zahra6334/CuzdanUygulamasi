using CuzdanUygulamasi.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalWallet.Services.Interfaces
{
    public interface ITaksitliOdemeService
    {
        // Tüm taksitli ödemeleri getir
        Task<List<TaksitliOdeme>> TaksitliOdemeleriGetirAsync();

        // Belirli bir ID'ye göre taksitli ödeme getir
        Task<TaksitliOdeme> TaksitliOdemeGetirAsync(int id);

        // Yeni bir taksitli ödeme oluştur
        Task<TaksitliOdeme> TaksitliOdemeEkleAsync(TaksitliOdeme odeme);

        // Mevcut bir taksitli ödemeyi güncelle
        Task<TaksitliOdeme> TaksitliOdemeGuncelleAsync(TaksitliOdeme odeme);

        // Belirli bir ID'ye sahip taksitli ödemeyi sil
        Task<bool> TaksitliOdemeSilAsync(int id);
    }
}
