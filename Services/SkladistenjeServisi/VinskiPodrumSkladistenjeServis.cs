using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Domain.Konstante;


namespace Services.SkladistenjeServisi
{
    public class VinskiPodrumSkladistenjeServis : ISkladistenjeServis
    {
        IPaletaRepozitorijum paletaRepozitorijum;
        ILoggerServis loggerServis;

        const int BRZA_ISPORUKA_BROJA_PALETA = IsporukaBrojaPaletaKonstante.BRZA_ISPORUKA_BROJA_PALETA;
        const int BRZA_ISPORUKA_TRAJANJE_SEKUNDE = 300;


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
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Vinski podrum: Broj paleta za isporuku nije validan.");
                    return new List<Paleta>();
                }

                List<Paleta> isporucene = new();
                int preostalo = brojPaleta;

                while (preostalo > 0)
                {
                    int tura = Math.Min(BRZA_ISPORUKA_BROJA_PALETA, preostalo);

                    var spremne = paletaRepozitorijum.PronadjiPaletePoStatusu(TipStatusaPalete.OTPREMLJENA).Take(tura).ToList();

                    if (spremne.Count == 0)
                    {
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Vinski podrum: Nema dostupnih OTPREMLJENIH paleta za isporuku.");
                        break; // da bi se dostavila isporucena, da ne vratimo praznu listu
                    }

                    // 0.3s po paleti
                    Task.Delay(spremne.Count * BRZA_ISPORUKA_TRAJANJE_SEKUNDE).Wait();

                    isporucene.AddRange(spremne);
                    preostalo -= spremne.Count;

                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Vinski podrum: Isporuceno {spremne.Count} paleta u turi (max 5). Preostalo: {preostalo}.");
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Vinski podrum: Ukupno isporuceno {isporucene.Count} paleta za zahtev {brojPaleta}.");

                return isporucene;
            }

            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Vinski podrum: Greska prilikom dostavljanja paleta servisu prodaje!");
                return new List<Paleta>();
            }
           
        }
    }
}
