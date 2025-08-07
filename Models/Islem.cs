using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuzdanUygulamasi.Models
{
    public enum IslemTipi
    {
        Gelir = 1,
        Gider = 2,
        Taksit = 3
    }
    public enum PeriyotTipi
    {
        Gunluk = 1,
        Haftalik = 2,
        Aylik = 3,
        Yillik = 4
    }
    public class Islem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kullanici")]
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }

        [Required]
        public decimal Tutar { get; set; }

        public string Aciklama { get; set; }

        [ForeignKey("Kategori")]
        public int? KategoriId { get; set; }
        public Kategori Kategori { get; set; }

        public DateTime Tarih { get; set; }

        public IslemTipi IslemTipi { get; set; }

        public bool TekrarliMi { get; set; } = false;

        public PeriyotTipi? Periyot { get; set; }

        [ForeignKey("TaksitliOdeme")]
        public int? TaksitOdemeId { get; set; }
        public TaksitliOdeme TaksitliOdeme { get; set; }
    }
}
