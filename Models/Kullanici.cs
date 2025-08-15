using System.ComponentModel.DataAnnotations;

namespace CuzdanUygulamasi.Models
{
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string AdSoyad { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string SifreHash { get; set; }

        public string? ProfiPic {  get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // İlişkiler
        public List<Kategori> Kategoriler { get; set; }
        public List<Islem> Islemler { get; set; }
        public List<TaksitliOdeme> TaksitliOdemeler { get; set; }
     
    }
}
