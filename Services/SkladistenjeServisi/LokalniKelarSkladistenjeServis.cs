using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Domain.Konstante;
using System.Linq;



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
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Lokalni Kelar: Broj paleta za isporuku nije validan.");
                    return new List<Paleta>();
                }

                List<Paleta> isporucene = new();
                int preostalo = brojPaleta;
                while (preostalo > 0)
                {
                    int tura = Math.Min(IsporukaBrojaPaletaKonstante.REDOVNA_ISPORUKA_BROJA_PALETA, preostalo);

                    var spremne = paletaRepozitorijum.PronadjiPaletePoStatusu(TipStatusaPalete.OTPREMLJENA).Take(tura).ToList();
                    var sveOtpremljene = paletaRepozitorijum.PronadjiPaletePoStatusu(TipStatusaPalete.OTPREMLJENA).ToList();
                    Console.WriteLine($"DEBUG: ukupno OTPREMLJENIH u bazi = {sveOtpremljene.Count}");
                    Console.WriteLine($"DEBUG: uzimam u turi = {tura}, spremne = {spremne.Count}");

                    if (spremne.Count == 0)
                    {
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Lokalni Kelar: Nema dostupnih OTPREMLJENIH paleta za isporuku.");
                        break; // da bi se dostavile isporucene palete, da ne vratimo praznu listu
                    }

                    // 1.8s po paleti
                    int ukupnoMs = spremne.Count * VremeIsporukaPaleta.REDOVNA_ISPORUKA_PALETA_SEKUNDE;
                    Task.Delay(ukupnoMs).Wait();

                    isporucene.AddRange(spremne);
                    preostalo -= spremne.Count;

                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Lokalni Kelar: Isporuceno {spremne.Count} paleta u turi (max 2). Preostalo: {preostalo}.");
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Lokalni Kelar: Ukupno isporuceno {isporucene.Count} paleta za zahtev {brojPaleta}.");

                return isporucene;
            }

            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Lokalni Kelar: Greska prilikom dostavljanja paleta servisu prodaje!");
                return new List<Paleta>();
            }

        }
    }
}
    