using Domain.Enumeracije;
using Domain.Modeli;
using Domain.PomocneMetode.VinovaLoza; 
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Services.VinogradarstvoServisi
{
    public class VinogradarstvoServis : IVinogradarstvoServis
    {
        IVinoveLozeRepozitorijum vinoveLozeRepozitorijum;
        ILoggerServis loggerServis;

        public VinogradarstvoServis(IVinoveLozeRepozitorijum vinoveLozeRepo, ILoggerServis logger)
        {
            this.vinoveLozeRepozitorijum = vinoveLozeRepo;
            loggerServis = logger;
        }

        public VinovaLoza PosadiNovuLozu(string nazivSorte)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nazivSorte))
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Pokušaj sadnje loze sa praznim nazivom sorte.");
                    return new VinovaLoza();
                }

                float nivoSecera = 15.0f;
                int godinaProizvodnje = 2025;
                string regionUzgoja = NasumicanRegionUzgojaHelper.GenerisiNasumicanRegionUzgoja();

                VinovaLoza novaViovaLoza = new VinovaLoza(0, nazivSorte, nivoSecera, godinaProizvodnje, regionUzgoja, FazaZrelosti.POSADJENA);
                novaViovaLoza = vinoveLozeRepozitorijum.DodajVinovuLozu(novaViovaLoza);

                if (novaViovaLoza.Id == 0)
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Sadnja loze nije uspela (sorta: {nazivSorte}).");
                else
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Zasađena nova loza (ID: {novaViovaLoza.Id}, sorta: {nazivSorte}, region: {novaViovaLoza.RegionUzgoja}).");

                return novaViovaLoza;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greška pri sadnji loze. ");
                return new VinovaLoza();
            }
        }

        public bool PromeniNivoSeceraZaProcenat(long vinovaLozaId, float procenat)
        {
            try
            {
                VinovaLoza loza = vinoveLozeRepozitorijum.PronadjiVinovuLozuPoId(vinovaLozaId);

                if (loza.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Ne postoji vinova loza sa tim ID-em.");
                    return false;
                }

                // povećaj/smanji za procenat: npr 10 => +10%, -5 => -5%
                float stariSecer = loza.NivoSecera;
                float noviSecer = stariSecer * (1.0f + (procenat / 100.0f));

                //ogranicenje secera
                if (noviSecer < 15.0f) 
                    noviSecer = 15.0f;

                if (noviSecer > 28.0f) 
                    noviSecer = 28.0f;

                loza.NivoSecera = noviSecer;

                bool ok = vinoveLozeRepozitorijum.AzurirajVinovuLozu(loza);

                if(ok)
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Procenat šećera je uspešno promenjen za {procenat} procenata, bio je {stariSecer}, a sada je {noviSecer}.");
                else
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Neuspešno ažuriranje loze pri promeni šećera (ID: {vinovaLozaId}).");

                return ok;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Neuspešno menjanje procenta šećera.");
                return false;
            }
        }

        public IEnumerable<VinovaLoza> OberiLozeJedneSorte(string nazivSorte, int kolicina)
        {
            try
            {
                List<VinovaLoza> rezultat = new List<VinovaLoza>();

                if (string.IsNullOrWhiteSpace(nazivSorte) || kolicina <= 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Neispravan zahtev za berbu (sorta: {nazivSorte}, kolicina: {kolicina}).");
                    return rezultat;
                }

                IEnumerable<VinovaLoza> sveTeSorte = vinoveLozeRepozitorijum.PronadjiVinoveLozePoNazivu(nazivSorte);

                int brojac = 0;
                foreach (VinovaLoza loza in sveTeSorte)
                {
                    if (brojac >= kolicina)
                        break;

                    loza.Faza = FazaZrelosti.OBRANA;
                    vinoveLozeRepozitorijum.AzurirajVinovuLozu(loza);

                    rezultat.Add(loza);
                    brojac++;
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Uspešno obrane {rezultat.Count()} vinove loze {nazivSorte}.");
                return rezultat;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Neuspešno branje {nazivSorte} vinove loze.");
                return new List<VinovaLoza>();
            }
        }
    }
}
