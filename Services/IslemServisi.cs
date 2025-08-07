using CuzdanUygulamasi.Models;

namespace CuzdanUygulamasi.Services
{
    public class IslemServisi
    {
        private readonly List<Islem> _islemler = new();

        public void IslemEkle(Islem islem)
        {
            _islemler.Add(islem);
        }

        public List<Islem> TumIslemleriGetir()
        {
            return _islemler;
        }

        public decimal ToplamGelirHesapla()
        {
            return _islemler
                .Where(i => i.IslemTipi == IslemTipi.Gelir)
                .Sum(i => i.Tutar);
        }

        public decimal ToplamGiderHesapla()
        {
            return _islemler
                .Where(i => i.IslemTipi == IslemTipi.Gider)
                .Sum(i => i.Tutar);
        }

        public decimal BakiyeHesapla()
        {
            return ToplamGelirHesapla() - ToplamGiderHesapla();
        }
    }
}
