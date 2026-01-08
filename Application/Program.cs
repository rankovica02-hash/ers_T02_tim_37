using Database.Repozitorijumi;
using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Presentation.Authentifikacija;
using Presentation.Meni;
using Services.AutenftikacioniServisi;
using Services.LoggerServisi;

namespace Loger_Bloger
{
    public class Program
    {
        public static void Main() //komentar 
        {
            // Baza podataka
            IBazaPodataka bazaPodataka = null; // TODO: Initialize the database with appropriate implementation

            // Repozitorijumi
            IKorisniciRepozitorijum korisniciRepozitorijum = new KorisniciRepozitorijum(bazaPodataka);



            // Servisi
            ILoggerServis loggerServis = new FileLoggerServis();
            IAutentifikacijaServis autentifikacijaServis = new AutentifikacioniServis(korisniciRepozitorijum, loggerServis); // TODO: Pass necessary dependencies
            // TODO: Add other necessary services

            // Ako nema nijednog korisnika u sistemu,dodati dva nova
            if (korisniciRepozitorijum.SviKorisnici().Count() == 0)
            {
                korisniciRepozitorijum.DodajKorisnika(new Korisnik("mare123", "sifra123", "Marko Markovic", TipKorisnika.GlavniEnolog));
                korisniciRepozitorijum.DodajKorisnika(new Korisnik("pera321", "sifra321", "Pera Peric", TipKorisnika.KelarMajstor));
                // TODO: Add initial users to the system
            }

            // Prezentacioni sloj
            AutentifikacioniMeni am = new AutentifikacioniMeni(autentifikacijaServis);
            Korisnik prijavljen = new Korisnik();

            while (am.TryLogin(out prijavljen) == false)
            {
                Console.WriteLine("Pogrešno korisničko ime ili lozinka. Pokušajte ponovo.");
            }

            Console.Clear();
            Console.WriteLine($"Uspešno ste prijavljeni kao: {prijavljen.ImePrezime} ({prijavljen.Uloga})");

            OpcijeMeni meni = new OpcijeMeni(); // TODO: Pass necessary dependencies
            meni.PrikaziMeni();
        }
    }
}
