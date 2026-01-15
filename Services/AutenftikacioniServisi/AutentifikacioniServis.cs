using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Services.AutenftikacioniServisi
{
    public class AutentifikacioniServis : IAutentifikacijaServis
    {
        IKorisniciRepozitorijum korisnickiRepozitorijum;
        ILoggerServis loggerServis;

        public AutentifikacioniServis(IKorisniciRepozitorijum repozitorijum, ILoggerServis logger)
        {
            korisnickiRepozitorijum = repozitorijum;
            loggerServis = logger;

        }   
        public (bool, Korisnik) Prijava(string korisnickoIme, string lozinka)
        {
            try
            {

                Korisnik pronadjen = korisnickiRepozitorijum.PronadjiKorisnikaPoKorisnickomImenu(korisnickoIme);
                if (pronadjen.KorisnickoIme != string.Empty && pronadjen.Lozinka == lozinka)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Korisnik '{korisnickoIme}' je uspešno prijavljen.");
                    return (true, pronadjen);
                }
                else
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Neuspešna prijava za korisnika '{korisnickoIme}'.");
                    return (false, new Korisnik());
                }

            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR,$"Greska tokom prijave korisnika '{korisnickoIme}'.");
                return (false, new Korisnik());
            }
          
        }

        public (bool, Korisnik) Registracija(Korisnik noviKorisnik)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(noviKorisnik.KorisnickoIme) || string.IsNullOrWhiteSpace(noviKorisnik.Lozinka) || string.IsNullOrWhiteSpace(noviKorisnik.ImePrezime))
                    return (false, new Korisnik());

                Korisnik postoji = korisnickiRepozitorijum.PronadjiKorisnikaPoKorisnickomImenu(noviKorisnik.KorisnickoIme);
                if (postoji.KorisnickoIme != string.Empty)
                {
                    loggerServis.EvidentirajDogadjaj(
                        TipEvidencije.WARNING,
                        $"Neuspešna registracija - korisnik '{noviKorisnik.KorisnickoIme}' već postoji."
                    );
                    return (false, new Korisnik());
                }

                Korisnik dodat = korisnickiRepozitorijum.DodajKorisnika(noviKorisnik);

                loggerServis.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    $"Korisnik '{noviKorisnik.KorisnickoIme}' je uspešno registrovan."
                );

                return (true, dodat);
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(
                    TipEvidencije.ERROR,
                    $"Greska tokom registracije korisnika '{noviKorisnik?.KorisnickoIme}'."
                );
                return (false, new Korisnik());
            }
        }
    }
}
