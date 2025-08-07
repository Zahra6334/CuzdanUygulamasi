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
        public int TaksitSayisi { get; set; }

        [Required]
        public float FaizOrani { get; set; }

        public int KalanTaksit { get; set; }

        public DateTime BaslangicTarihi { get; set; }

        public DateTime BitisTarihi { get; set; }
       
    }
}
