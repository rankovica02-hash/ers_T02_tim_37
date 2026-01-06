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
