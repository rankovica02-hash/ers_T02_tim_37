using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Servisi;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.ProdajaServisi
{
    public class ProdajaVinaServis : IProdajaVinaServis
    {
        IFakturaRepozitorijum fakturaRepozitorijum;
        ILoggerServis loggerServis;
        IKatalogVinaRepozitorijum katalogVinaRepozitorijum;
        IVinaRepozitorijum vinaRepozitorijum;
        ISkladistenjeServis skladistenjeServis;

        const int FLASA_PO_PALETI = 20;

        public ProdajaVinaServis(
            IFakturaRepozitorijum fakturaRepozitorijum,
            IKatalogVinaRepozitorijum katalogVinaRepozitorijum,
            IVinaRepozitorijum vinaRepozitorijum,
            ISkladistenjeServis skladistenjeServis,
            ILoggerServis loggerServis)
        {
            this.fakturaRepozitorijum = fakturaRepozitorijum;
            this.katalogVinaRepozitorijum = katalogVinaRepozitorijum;
            this.vinaRepozitorijum = vinaRepozitorijum;
            this.skladistenjeServis = skladistenjeServis;
            this.loggerServis = loggerServis;
        }

        public IEnumerable<Vino> PrikaziKatalog()
        {
            try
            {
                var katalog = katalogVinaRepozitorijum
                    .PronadjiSveKataloge()
                    .FirstOrDefault(k => k.VinaIds != null && k.VinaIds.Count > 0);

                if (katalog == null || katalog.VinaIds == null || katalog.VinaIds.Count == 0)
                    return Enumerable.Empty<Vino>();

                return katalog.VinaIds
                .Select(id => vinaRepozitorijum.PronadjiVinoPoId(id))
                .Where(v => v != null && v.Id != 0)
                .Select(v => v!);
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Prodaja: Neuspesan prikaz kataloga.");
                return Enumerable.Empty<Vino>();
            }
        }

        public Faktura ProdajaIzKataloga(long vinoId, int brojFlasa, TipProdaje tipProdaje, NacinPlacanja nacinPlacanja)
        {
            try
            {
                if (brojFlasa <= 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Prodaja: Nevalidan broj flasa.");
                    return new Faktura();
                }

                var katalog = katalogVinaRepozitorijum
                    .PronadjiSveKataloge()
                    .FirstOrDefault(k => k.VinaIds != null && k.VinaIds.Count > 0);

                if (katalog == null || katalog.VinaIds == null || !katalog.VinaIds.Contains(vinoId))
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Prodaja: Vino {vinoId} nije u katalogu.");
                    return new Faktura();
                }

                var vino = vinaRepozitorijum.PronadjiVinoPoId(vinoId);
                if (vino == null || vino.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Prodaja: Vino {vinoId} ne postoji.");
                    return new Faktura();
                }

                int brojPaleta = (int)Math.Ceiling(brojFlasa / (double)FLASA_PO_PALETI);

                var palete = skladistenjeServis.IsporuciPaleteServisuProdaje(brojPaleta);
                if (palete.Count < brojPaleta)
                {
                    loggerServis.EvidentirajDogadjaj(
                        TipEvidencije.WARNING,
                        $"Prodaja: Nema dovoljno paleta. Trazeno {brojPaleta}, dobijeno {palete.Count}."
                    );
                    return new Faktura();
                }

                var faktura = new Faktura
                {
                    TipProdaje = tipProdaje,
                    NacinPlacanja = nacinPlacanja
                };

                string nazivVina = GetNazivVina(vino);

                faktura.Stavke.Add(new StavkeFakture(vinoId, nazivVina, brojFlasa, 0m));

                fakturaRepozitorijum.DodajFakturu(faktura);

                loggerServis.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    $"Prodaja: Faktura {faktura.Id} | Vino={vinoId} | Flasa={brojFlasa} | Paleta={brojPaleta} | Ukupno={faktura.UkupanIznos}"
                );

                return faktura;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Prodaja: Greska tokom prodaje vina.");
                return new Faktura();
            }
        }

        public IEnumerable<Faktura> PregledSvihFaktura()
        {
            try
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, "Prodaja: Pregled svih faktura.");
                return fakturaRepozitorijum.SveFakture();
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Prodaja: Neuspesan pregled faktura.");
                return Enumerable.Empty<Faktura>();
            }
        }

        private static string GetNazivVina(Vino vino)
        {
            var p1 = vino.GetType().GetProperty("Naziv");
            if (p1?.GetValue(vino) is string s1) return s1;

            var p2 = vino.GetType().GetProperty("NazivVina");
            if (p2?.GetValue(vino) is string s2) return s2;

            return "Nepoznato vino";
        }
    }
}
