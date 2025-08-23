using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuzdanUygulamasi.Models
{
    public class OdemeTaksiti
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaksitliOdemeId { get; set; }

        [ForeignKey("TaksitliOdemeId")]
        public TaksitliOdeme TaksitliOdeme { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OdenenTutar { get; set; }

        [Required]
        public DateTime OdemeTarihi { get; set; }

        [Required]
        [StringLength(50)]
        public string OdemeYontemi { get; set; }
  
        public bool OdendiMi { get; internal set; }
        public DateTime SonOdemeTarihi { get; internal set; }
        public int? IslemId { get; set; }
        public Islem Islem { get; set; }

    }
}
