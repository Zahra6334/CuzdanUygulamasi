using CuzdanUygulamasi.Controllers;
using CuzdanUygulamasi.Models;

using Microsoft.EntityFrameworkCore;

namespace CuzdanUygulamasi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Islem> Islemler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<TaksitliOdeme> TaksitliOdemeler { get; set; }

        internal async Task<string?> GetByIdAsync(int id, int kullaniciId)
        {
            throw new NotImplementedException();
        }

        internal async Task<string?> GetByUserIdAsync(int kullaniciId)
        {
            throw new NotImplementedException();
        }

        internal async Task<bool> TaksitliOdemeyiGuncelleAsync(TaksitliOdeme odeme, int kullaniciId)
        {
            throw new NotImplementedException();
        }

        internal async Task TaksitliOdemeyiSilAsync(int id, int kullaniciId)
        {
            throw new NotImplementedException();
        }

        internal async Task TaksitOdemeYapAsync(int id, int kullaniciId)
        {
            throw new NotImplementedException();
        }

        internal async Task YeniTaksitliOdemeEkleAsync(TaksitliOdeme odeme)
        {
            throw new NotImplementedException();
        }

        public static implicit operator ApplicationDbContext(TaksitliOdemeController v)
        {
            throw new NotImplementedException();
        }
    }
}
