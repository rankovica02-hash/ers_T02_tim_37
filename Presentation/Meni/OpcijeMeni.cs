using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Servisi;
namespace Presentation.Meni
{
    public class OpcijeMeni
    {
        IPaletaServis paletaServis;
        ISkladistenjeServis skladistenjeServis;
        IProdajaVinaServis prodajaServis;
        public OpcijeMeni(IPaletaServis paleta, ISkladistenjeServis skladistenjeServ, IProdajaVinaServis prodaja)
        {
            paletaServis = paleta;
            skladistenjeServis = skladistenjeServ;
            prodajaServis = prodaja;
        }
        public void PrikaziMeni()
        {
            Console.WriteLine("\n============================================ Meni ===========================================");

            bool kraj = false;
            while (!kraj)
            {
                // TODO: Prikaz opcija menija
                Console.WriteLine("\n1. Kreiraj paletu");
                Console.WriteLine("2. Prikaži palete");
                Console.WriteLine("3. Zahtev za isporuku palete servisu prodaje");
                Console.WriteLine("4. Otpremi paletu (UPAKOVANA -> OTPREMLJENA)");
                Console.WriteLine("5. Prodaja vina (katalog -> faktura)"); //
                Console.WriteLine("0. Izlaz");
                Console.WriteLine("Opcija: ");

                string? opcija = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(opcija))
                    continue;

                switch(opcija[0])
                {
                    case '1':
                        KreiranjePalete();
                        break;

                    case '2':
                        PrikazPalete();
                        break;

                    case '3':
                        ZahtevIsporukeServisuProdaje();
                        break;

                    case '4':
                        OtpremiPaleteMeni();
                        break;

                    case '5':
                        ProdajaVinaMeni();
                        break;

                    case '0':
                        kraj = true;
                        break;
                }
            }
        }

        private void KreiranjePalete()
        {
            Console.WriteLine("Unesite adresu odredišta: ");
            string adresa = Console.ReadLine() ?? "";

            Console.WriteLine("Unesite ID vinskog podruma: ");
            long.TryParse(Console.ReadLine(), out long podrumId);

            Paleta paleta = paletaServis.KreiranjePalete(adresa, podrumId);

            if (paleta.Id != 0)
                Console.WriteLine($"Kreirana paleta: {paleta.Sifra}");
            else
                Console.WriteLine("Greška pri kreiranju palete.");
        }

        private void PrikazPalete()
        {
            IEnumerable<Paleta> palete = paletaServis.PrikazSvihPaleta();
            Console.WriteLine("\n=== PALETE ===");
            foreach(Paleta p in palete)
            {
                Console.WriteLine($"Šifra: {p.Sifra},  odredište: {p.AdresaOdredista}, podrumId: {p.VinskiPodrumId}, status: {p.Status}");
            }
        }

        private void OtpremiPaleteMeni()
        {
            Console.Write("Unesite broj paleta za otpremu: ");
            if (!int.TryParse(Console.ReadLine(), out int broj) || broj <= 0)
            {
                Console.WriteLine("Neispravan broj.");
                return;
            }

            var otpremljene = paletaServis.OtpremiPalete(broj);

            if (otpremljene.Count == 0)
            {
                Console.WriteLine("Nema paleta koje mogu da se otpreme.");
                return;
            }

            Console.WriteLine($"Otpremljeno paleta: {otpremljene.Count}");
            foreach (var p in otpremljene)
            {
                Console.WriteLine($"- {p.Sifra} (Id={p.Id}, Status={p.Status})");
            }
        }



        public void ZahtevIsporukeServisuProdaje()
        {
            Console.WriteLine("Unesite broj paleta za isporuku:");
            string? unos = Console.ReadLine();
            if (!int.TryParse(unos, out int brojPaleta) || brojPaleta <= 0)
            {
                Console.WriteLine("Unos nije validan. Molim Vas unesite ceo pozitivan broj.");
                return;
            }

            var palete = skladistenjeServis.IsporuciPaleteServisuProdaje(brojPaleta);

            if (palete.Count == 0)
            {
                Console.WriteLine("Nema OTPREMLJENIH paleta za isporuku (0 isporuceno).");
                return;
            }

            Console.WriteLine($"Uspesna isporuka! Isporuceno: {palete.Count}");
            foreach (var p in palete)
                Console.WriteLine($"- {p.Sifra} (Id={p.Id}, Status={p.Status})");
        }

        private void ProdajaVinaMeni()
        {
            var katalog = prodajaServis.PrikaziKatalog().ToList();
            if (katalog.Count == 0)
            {
                Console.WriteLine("Katalog je prazan.");
                return;
            }

            Console.WriteLine("\n=== KATALOG VINA ===");
            foreach (var v in katalog)
            {
                // ako se u Vino klasi drugačije zovu polja, ovde samo zameni
                Console.WriteLine($"Id={v.Id} | {v.Naziv}");
            }

            Console.Write("Unesite ID vina: ");
            if (!long.TryParse(Console.ReadLine(), out long vinoId))
            {
                Console.WriteLine("Neispravan ID vina.");
                return;
            }

            Console.Write("Unesite broj flasa: ");
            if (!int.TryParse(Console.ReadLine(), out int brojFlasa) || brojFlasa <= 0)
            {
                Console.WriteLine("Neispravan broj flasa.");
                return;
            }

            // TIP PRODAJE
            TipProdaje tipProdaje = UcitajTipProdaje();
            // NACIN PLACANJA
            NacinPlacanja nacinPlacanja = UcitajNacinPlacanja();

            var faktura = prodajaServis.ProdajaIzKataloga(vinoId, brojFlasa, tipProdaje, nacinPlacanja);

            if (faktura.Stavke.Count == 0)
            {
                Console.WriteLine("Prodaja nije uspela.");
                return;
            }

            Console.WriteLine($"\n=== FAKTURA {faktura.Id} ===");
            Console.WriteLine($"Datum: {faktura.DatumIzdavanja}");
            Console.WriteLine($"Tip prodaje: {faktura.TipProdaje}");
            Console.WriteLine($"Način plaćanja: {faktura.NacinPlacanja}");

            foreach (var s in faktura.Stavke)
                Console.WriteLine($"- {s.NazivVina} x{s.Kolicina} = {s.UkupnaCena}");

            Console.WriteLine($"UKUPNO: {faktura.UkupanIznos}");
        }

        private static TipProdaje UcitajTipProdaje()
        {
            while (true)
            {
                Console.WriteLine("\nIzaberite tip prodaje:");
                Console.WriteLine("1 - Restoranska prodaja");
                Console.WriteLine("2 - Diskont pića");
                Console.Write("Opcija: ");

                string? izbor = Console.ReadLine();
                if (izbor == "1") return TipProdaje.RESTORANSKA_PRODAJA;
                if (izbor == "2") return TipProdaje.DISKONT_PICA;

                Console.WriteLine("Neispravan izbor. Pokušaj ponovo.");
            }
        }

        private static NacinPlacanja UcitajNacinPlacanja()
        {
            while (true)
            {
                Console.WriteLine("\nIzaberite način plaćanja:");
                Console.WriteLine("1 - Gotovina");
                Console.WriteLine("2 - Predračun");
                Console.WriteLine("3 - Gotovinski račun");
                Console.Write("Opcija: ");

                string? izbor = Console.ReadLine();
                if (izbor == "1") return NacinPlacanja.GOTOVINA;
                if (izbor == "2") return NacinPlacanja.PREDRACUN;
                if (izbor == "3") return NacinPlacanja.GOTOVINSKI_RACUN;

                Console.WriteLine("Neispravan izbor. Pokušaj ponovo.");
            }
        }

    }
}
