using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Services.PaletaServisi
{
    public class PaletaServis : IPaletaServis
    {
        IPaletaRepozitorijum paletaRepozitorijum;
        ILoggerServis loggerServis;

        public PaletaServis(IPaletaRepozitorijum paletaRepo,ILoggerServis logger)
        {
            paletaRepozitorijum = paletaRepo;
            loggerServis = logger;
        }

        public Paleta KreiranjePalete(string adresaOdredista, long vinskiPodrumId)
        {
            try
            {
                Paleta novaPaleta = new Paleta
                {
                    Id = 0,
                    Sifra = string.Empty,
                    AdresaOdredista = adresaOdredista,
                    VinskiPodrumId = vinskiPodrumId,
                    VinaIds = new List<long>(),
                    Status = TipStatusaPalete.UPAKOVANA
                };

                novaPaleta = paletaRepozitorijum.DodajPaletu(novaPaleta);
                
                if(novaPaleta.Id != 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Kreirana paleta: {novaPaleta.Sifra}");
                    return novaPaleta;
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Neuspešno kreiranje palete.");
                return new Paleta();
            }

            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Izuzetak pri kreiranju palete.");
                return new Paleta();
            }
        }

        public IEnumerable<Paleta> PrikazPaletaPoStatusu(TipStatusaPalete status)
        {
            try
            {
                return paletaRepozitorijum.PronadjiPaletePoStatusu(status);
            }
            catch
            {
                return [];
            }
        }

        public List<Paleta> OtpremiPalete(int brojPaleta)
        {
            if (brojPaleta <= 0)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Otpremanje paleta: broj paleta nije validan.");
                return new List<Paleta>();
            }

            try
            {
                List<Paleta> otpremljene = new();

                var upakovane = paletaRepozitorijum.PronadjiPaletePoStatusu(TipStatusaPalete.UPAKOVANA).Take(brojPaleta).ToList();

                if (upakovane.Count == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING,"Otpremanje paleta: nema UPAKOVANIH paleta.");
                    return new List<Paleta>();
                }

                foreach (var paleta in upakovane)
                {
                    paleta.Status = TipStatusaPalete.OTPREMLJENA;
                    paletaRepozitorijum.AzurirajPaletu(paleta);
                    otpremljene.Add(paleta);
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,$"Otpremanje paleta: otpremljeno {otpremljene.Count} paleta.");

                return otpremljene;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR,"Greska pri otpremanju paleta.");
                return new List<Paleta>();
            }
        }



        public IEnumerable<Paleta> PrikazSvihPaleta()
        {
            try
            {
                return paletaRepozitorijum.PronadjiSvePalete();
            }
            catch
            {
                return [];  
            }
        }
    }
}
