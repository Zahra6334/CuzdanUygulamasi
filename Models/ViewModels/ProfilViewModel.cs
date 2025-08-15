using System;
using System.Collections.Generic;

namespace CuzdanUygulamasi.Models.ViewModels
{
    public class ProfilViewModel
    {
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; }
        public string Email { get; set; }
        public string ProfilPic {  get; set; }
        public decimal ToplamGelir { get; set; }
        public decimal ToplamGider { get; set; }
        public decimal ToplamBakiye => ToplamGelir - ToplamGider;

        public int IslemSayisi { get; set; }
        public int TaksitSayisi { get; set; }
        public int KategoriSayisi { get; set; }

        public List<Islem> SonIslemler { get; set; }
        public string? ProfiPic { get; set; }
        public List<TaksitliOdeme> AktifTaksitler { get; set; } = new List<TaksitliOdeme>();
    }
}
