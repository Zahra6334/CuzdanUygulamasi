namespace CuzdanUygulamasi.Models
{
    public class RegisterViewModel
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public string SifreTekrar { get; set; }
        public bool KvkkOnay { get; set; }
    }
}
