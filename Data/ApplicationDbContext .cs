using CuzdanUygulamasi.Models;
using Microsoft.EntityFrameworkCore;

namespace CuzdanUygulamasi.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Bu constructor çok önemli!
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Islem> Islemler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<TaksitliOdeme> TaksitliOdemeler { get; set; }
        public DbSet<OdemeTaksiti> OdemeTaksitleri { get;  set; }

        public DbSet<Bildirim> Bildirimler { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaksitliOdeme>()
                .Property(t => t.ToplamTutar)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OdemeTaksiti>()
                .Property(o => o.OdenenTutar)
                .HasPrecision(18, 2);

            // OdemeTaksiti -> Islem ilişkisi, cascade delete kapalı
            modelBuilder.Entity<OdemeTaksiti>()
                 .HasOne(ot => ot.Islem)
                 .WithMany(i => i.OdemeTaksitleri)
                 .HasForeignKey(ot => ot.IslemId)
                 .OnDelete(DeleteBehavior.Restrict); // NO ACTION
        }


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
    }
}
