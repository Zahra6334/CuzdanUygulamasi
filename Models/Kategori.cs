using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuzdanUygulamasi.Models
{
    public class Kategori
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Ad { get; set; }

        // Kullanıcıya ait
        [ForeignKey("Kullanici")]
        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
    }
}
