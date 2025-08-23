namespace CuzdanUygulamasi.Models
{
    public class Bildirim
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string Mesaj { get; set; }
        public DateTime Tarih { get; set; }
        public bool OkunduMu { get; set; } = false;

        public Kullanici Kullanici { get; set; } // navigation property
    }
}
