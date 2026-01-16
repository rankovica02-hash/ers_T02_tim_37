using Database.BazaPodataka;
using Database.Repozitorijumi;
using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Domain.PomocneMetode.Vino;
using Domain.PomocneMetode.VinovaLoza;
using Domain.Konstante;
using Presentation.Authentifikacija;
using Presentation.Meni;
using Services.AutenftikacioniServisi;
using Services.LoggerServisi;
using Services.PaletaServisi;
using Services.VinogradarstvoServisi;
using Services.SkladistenjeServisi;

namespace Loger_Bloger
{
    public class Program
    {
        public static void Main() //komentar 
        {
            // Baza podataka
            IBazaPodataka bazaPodataka = new XMLBazaPodataka(); // TODO: Initialize the database with appropriate implementation

            // Repozitorijumi
            IKorisniciRepozitorijum korisniciRepozitorijum = new KorisniciRepozitorijum(bazaPodataka);
            IVinoveLozeRepozitorijum vinoveLozeRepozitorijum = new VinoveLozeRepozitorijum(bazaPodataka);
            IVinaRepozitorijum vinaRepozitorijum = new VinaRepozitorijum(bazaPodataka);
            IPaletaRepozitorijum paletaRepozitorijum = new PaletaRepozitorijum(bazaPodataka);
            IVinskiPodrumRepozitorijum vinskiPodrumRepozitorijum = new VinskiPodrumiRepozitorijum(bazaPodataka);
            IKatalogVinaRepozitorijum katalogVinaRepozitorijum = new KataloziVinaRepozitorijum(bazaPodataka);
           

            // Servisi
            ILoggerServis loggerServis = new FileLoggerServis();
            IAutentifikacijaServis autentifikacijaServis = new AutentifikacioniServis(korisniciRepozitorijum, loggerServis); // TODO: Pass necessary dependencies
            // TODO: Add other necessary services
            IPaletaServis paletaServis = new PaletaServis(paletaRepozitorijum,loggerServis);
            IVinogradarstvoServis vinogradarstvoServis = new VinogradarstvoServis(vinoveLozeRepozitorijum, loggerServis);
            ISkladistenjeServis skladistenjeServis;

            // Ako nema nijednog korisnika u sistemu,dodati dva nova
            if (korisniciRepozitorijum.SviKorisnici().Count() == 0)
            {
                korisniciRepozitorijum.DodajKorisnika(new Korisnik("mare123", "sifra123", "Marko Markovic", TipKorisnika.GlavniEnolog));
                korisniciRepozitorijum.DodajKorisnika(new Korisnik("pera321", "sifra321", "Pera Peric", TipKorisnika.KelarMajstor));
                // TODO: Add initial users to the system
            }

            // Katalog vina (ako nema nijednog kataloga)
            if(katalogVinaRepozitorijum.PronadjiSveKataloge().Count() == 0)
            {
                katalogVinaRepozitorijum.DodajKatalogVina(new KatalogVina { Naziv = "Katalog - dostupna vina", VinaIds = new List<long>() });
            }

            // Vinski podrumi
            if(vinskiPodrumRepozitorijum.PronadjiSveVinskePodrume().Count() == 0)
            {
                vinskiPodrumRepozitorijum.DodajVinskiPodrum(new VinskiPodrum
                {
                    Naziv = "Podrum A",
                    TemperaturaSkladistenja = 12.0,
                    MaksimalanBrojPaleta = 10
                });
            }

            // Prezentacioni sloj
            AutentifikacioniMeni am = new AutentifikacioniMeni(autentifikacijaServis);
            Korisnik prijavljen = new Korisnik();

            while (am.TryLogin(out prijavljen) == false)
            {
                if (prijavljen == null)
                    return;
                    Console.WriteLine("Pogrešno korisničko ime ili lozinka. Pokušajte ponovo.");
            }

            Console.Clear();
            Console.WriteLine($"Uspešno ste prijavljeni kao: {prijavljen.ImePrezime} ({prijavljen.Uloga})");

            if (prijavljen.Uloga == TipKorisnika.GlavniEnolog)
            {
                skladistenjeServis = new VinskiPodrumSkladistenjeServis(paletaRepozitorijum, loggerServis);
            }
            else 
            {
                skladistenjeServis = new LokalniKelarSkladistenjeServis(paletaRepozitorijum, loggerServis);
            }

            OpcijeMeni meni = new OpcijeMeni(paletaServis, skladistenjeServis); // TODO: Pass necessary dependencies
            meni.PrikaziMeni();
        }
    }
}
