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
    }
}
