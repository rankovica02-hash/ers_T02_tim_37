using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;


namespace Services.SkladistenjeServisi
{
    public class VinskiPodrumSkladistenjeServis : ISkladistenjeServis
    {
        IPaletaRepozitorijum paletaRepozitorijum;
        ILoggerServis loggerServis;

        public VinskiPodrumSkladistenjeServis(IPaletaRepozitorijum paletaRepo, ILoggerServis logger)
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
                int maxPoIsporuci = 5;
                int kolicina = Math.Min(brojPaleta, maxPoIsporuci);

                var palete = paletaRepozitorijum.PronadjiPaletePoStatusu(TipStatusaPalete.OTPREMLJENA).Take(kolicina).ToList();
                if(palete.Count() == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Otpremljene palete nisu dostupne u vinskom podrumu.");
                }
                else
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Vinski podrum isporucuje palete servisu prodaje. Broj paleta: {palete.Count()}.");
                }
                return palete;
            }

            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Greska prilikom dostavljanja paleta servisu prodaje!");
                return new List<Paleta>();
            }
           
        }
    }
}
