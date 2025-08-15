using CuzdanUygulamasi.Models;
namespace CuzdanUygulamasi.Models.ViewModels
{
    public class AnalizViewModel
    {
        public List<AylikGelirGider> AylikGelirGider { get; set; }
        public List<KategoriDagilimi> KategoriDagilimi { get; set; }
        public List<TaksitOzeti> TaksitOzeti { get; set; }
    }

    public class AylikGelirGider
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public IslemTipi IslemTipi { get; set; }
        public decimal Toplam { get; set; }
    }

    public class KategoriDagilimi
    {
        public string KategoriAdi { get; set; }
        public decimal Toplam { get; set; }
    }

    public class TaksitOzeti
    {
        //public TaksitDurum Durum { get; set; }
        public decimal ToplamTutar { get; set; }
    }
}

