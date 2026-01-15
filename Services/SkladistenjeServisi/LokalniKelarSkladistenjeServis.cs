using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;


namespace Services.SkladistenjeServisi
{
    public class LokalniKelarSkladistenjeServis : ISkladistenjeServis
    {
        IPaletaRepozitorijum paletaRepozitorijum;
        ILoggerServis loggerServis;

        public LokalniKelarSkladistenjeServis(IPaletaRepozitorijum paletaRepo, ILoggerServis logger)
        {
            paletaRepozitorijum = paletaRepo;
            loggerServis = logger;
        }

        public List<Paleta> IsporuciPaleteServisuProdaje(int brojPaleta)
        {
            try
            {
                if (brojPaleta <= 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Broj paleta za isporuku nije validan.");
                    return new List<Paleta>();
                }
                int maxPoIsporuci = 2;
                int kolicina = Math.Min(brojPaleta, maxPoIsporuci);

                var palete = paletaRepozitorijum.PronadjiPaletePoStatusu(TipStatusaPalete.OTPREMLJENA).Take(kolicina).ToList();
                if (palete.Count() == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Otpremljene palete nisu dostupne kod lokalnog kelara.");
                }
                else
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Lokalni kelar isporucuje palete servisu prodaje. Broj paleta: {palete.Count()}.");
                }
                return palete;
            }

            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Greska prilikom dostavljanja paleta kod lokalnog kelara servisu prodaje!");
                return new List<Paleta>();
            }

        }
    }
}
