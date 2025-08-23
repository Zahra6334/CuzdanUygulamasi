using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuzdanUygulamasi.Models
{
    public class TaksitliOdeme
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kullanici")]
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }

        [Required]
        public decimal ToplamTutar { get; set; }

        [Required]
        [Range(1, 120)]
        public int TaksitSayisi { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal FaizOrani { get; set; }

        public int KalanTaksit { get; set; }

        public DateTime BaslangicTarihi { get; set; } = DateTime.Now;

        public DateTime SonOdemeTarihi => BaslangicTarihi.AddMonths(TaksitSayisi);

        [NotMapped]
        public decimal AylikTaksit => (ToplamTutar * (1 + FaizOrani / 100)) / TaksitSayisi;

        public DateTime BitisTarihi { get; internal set; }
        public ICollection<OdemeTaksiti> OdemeTaksitleri { get; set; }

    }
}
