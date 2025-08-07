using CuzdanUygulamasi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using CuzdanUygulamasi.Data;

namespace CuzdanUygulamasi.Services
{
    public class TaksitliOdemeServisi
    {
        private readonly ApplicationDbContext _veriTabani;

        public TaksitliOdemeServisi(ApplicationDbContext veriTabani)
        {
            _veriTabani = veriTabani;
        }

        public List<TaksitliOdeme> TumTaksitliOdemeleriGetir()
        {
            return _veriTabani.TaksitliOdemeler.ToList();
        }

        public TaksitliOdeme IdIleTaksitliOdemeGetir(int id)
        {
            return _veriTabani.TaksitliOdemeler.FirstOrDefault(x => x.Id == id);
        }

        public void YeniTaksitliOdemeEkle(TaksitliOdeme odeme)
        {
            odeme.BaslangicTarihi = DateTime.Now;
            _veriTabani.TaksitliOdemeler.Add(odeme);
            _veriTabani.SaveChanges();
        }

        public void TaksitliOdemeyiGuncelle(TaksitliOdeme guncelOdeme)
        {
            var eskiOdeme = _veriTabani.TaksitliOdemeler.FirstOrDefault(x => x.Id == guncelOdeme.Id);

            if (eskiOdeme != null)
            {
                
                eskiOdeme.ToplamTutar = guncelOdeme.ToplamTutar;
                eskiOdeme.TaksitSayisi = guncelOdeme.TaksitSayisi;
                eskiOdeme.BaslangicTarihi = guncelOdeme.BaslangicTarihi;
                _veriTabani.SaveChanges();
            }
        }

        public void TaksitliOdemeyiSil(int id)
        {
            var silinecek = _veriTabani.TaksitliOdemeler.FirstOrDefault(x => x.Id == id);
            if (silinecek != null)
            {
                _veriTabani.TaksitliOdemeler.Remove(silinecek);
                _veriTabani.SaveChanges();
            }
        }
    }
}
