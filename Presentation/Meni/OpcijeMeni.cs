using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Servisi;
using Domain.PomocneMetode.Vino;
namespace Presentation.Meni
{
    public class OpcijeMeni
    {
        IProdajaVinaServis prodajaServis;
        Korisnik prijavljen;
        
        
        List<long> faktureUTekuccojPrijavi = new();
        public OpcijeMeni(IProdajaVinaServis prodajaServis, Korisnik prijavljen)
        {
            this.prodajaServis = prodajaServis;
            this.prijavljen = prijavljen;
        }
        public void PrikaziMeni()
        {
            bool kraj = false;
            while (!kraj)
            {
                Console.WriteLine("\n============================================ Meni ===========================================");
                Console.WriteLine("\n1. Katalog vina");
                Console.WriteLine("2. Prodaja vina");
                if (prijavljen.Uloga == TipKorisnika.GlavniEnolog)
                    Console.WriteLine("3. Pregled svih faktura");
                else
                    Console.WriteLine("3. Pregled fakture");
                Console.WriteLine("0. Izlaz");
                Console.WriteLine("Opcija: ");

                string? opcija = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(opcija))
                    continue;

                switch(opcija[0])
                {
                    case '1':
                        PrikaziKatalog();
                        break;

                    case '2':
                        ProdajaMeni();
                        break;

                    case '3':
                        PregledFakture();
                        break;

                    case '0':
                        kraj = true;
                        break;

                    default:
                        Console.WriteLine("Nepoznata opcija.");
                        break;
                }
            }
        }

        private void PrikaziKatalog()
        {
            var ponuda = NasumicanNazivVinaHelper.Nazivi;
            Console.WriteLine("+---------------------------------------------+");
            Console.WriteLine("|               KATALOG VINA                  |");
            Console.WriteLine("+---------------------------------------------+");
            Console.WriteLine("Svako vino je dostupno u sledećim kategorijama: STOLNO / KVALITETNO / PREMIJUM \n");
            if (ponuda == null || ponuda.Count == 0)
            {
                Console.WriteLine("Katalog je prazan.");
                Console.WriteLine("=================================================");
                return;
            }

            for(int i = 0; i < ponuda.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {ponuda[i]}");
            }
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("* Napomena *");
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("Cene vina zavise od kategorije, broja flaša i zapremine.");
            Console.WriteLine("Dostupne zapremine boca su u 0.75L i 1.5L.");
            Console.WriteLine("------------------------------------------------------------");
        }

        private void ProdajaMeni()
        {
            Console.Write("Unesite naziv vina: ");
            string nazivSorte = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(nazivSorte))
            {
                Console.WriteLine("Naziv vina ne sme biti prazan.");
                return;
            }

            var ponuda = NasumicanNazivVinaHelper.Nazivi;
            bool postoji = ponuda.Any(x => string.Equals(x, nazivSorte, StringComparison.OrdinalIgnoreCase));
            if (!postoji)
            {
                Console.WriteLine("Nepoznat naziv vina. Morate uneti jedan od naziva iz kataloga (opcija 1).");
                return;
            }
            KategorijaVina kategorija = UnesiKategoriju();

            Console.Write("Unesite broj flasa: ");
            if (!int.TryParse(Console.ReadLine(), out int brojFlasa) || brojFlasa <= 0)
            {
                Console.WriteLine("Broj flasa nije validan.");
                return;
            }

            Console.Write("Unesite zapreminu flase u litrima (0.75 ili 1.5): ");
            string zapStr = (Console.ReadLine() ?? "").Trim();
            zapStr = zapStr.Replace(',', '.');

            if (!double.TryParse(zapStr, System.Globalization.CultureInfo.InvariantCulture, out double zapremina) || zapremina <= 0)
            {
                Console.WriteLine("Zapremina nije validna.");
                return;
            }

            double z = Math.Round(zapremina, 2);
            if (z != 0.75 && z != 1.50)
            {
                Console.WriteLine("Zapremina nije validna. Dozvoljeno: 0.75 ili 1.5.");
                return;
            }
            zapremina = z;

            TipProdaje tipProdaje = UnesiTipProdaje();
            NacinPlacanja nacinPlacanja = UnesiNacinPlacanja();

            Console.Write("Unesite adresu odredista: ");
            string adresa = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(adresa))
            {
                Console.WriteLine("Adresa ne sme biti prazna.");
                return;
            }

            Faktura faktura = prodajaServis.Prodaj(nazivSorte.Trim(), kategorija, brojFlasa, zapremina, adresa.Trim(), tipProdaje, nacinPlacanja);

            if (faktura == null || faktura.Id == 0)
            {
                Console.WriteLine("Prodaja nije uspela.");
                return;
            }
            faktureUTekuccojPrijavi.Add(faktura.Id);
            Console.WriteLine($"Prodaja uspesna! Kreirana faktura (Id): {faktura.Id}");
        }

        private void PregledFakture()
        {
            var sveFakture = prodajaServis.PregledSvihFaktura().ToList();
            Console.WriteLine("\n=== FAKTURE ===");

            if (sveFakture.Count == 0)
            {
                Console.WriteLine("Nema faktura (još nije bilo prodaje).");
                return;
            }


            if (prijavljen.Uloga == TipKorisnika.GlavniEnolog)
            {
                foreach (var f in sveFakture)
                {
                    Console.WriteLine($"Id: {f.Id}, Datum: {f.DatumIzdavanja:dd.MM.yyyy HH:mm}, Tip: {f.TipProdaje}, Plaćanje: {f.NacinPlacanja}");
                }
                return;
            }

            if (faktureUTekuccojPrijavi.Count == 0)
            {
                Console.WriteLine("Nema dostupnih faktura.");
                return;
            }
            var mojeFakture = sveFakture.Where(f => faktureUTekuccojPrijavi.Contains(f.Id)).ToList();

            if (mojeFakture.Count == 0)
            {
                Console.WriteLine("Nema dostupnih faktura.");
                return;
            }

            foreach (var f in mojeFakture)
            {
                Console.WriteLine($"Id: {f.Id}, Datum: {f.DatumIzdavanja:dd.MM.yyyy HH:mm}, Tip: {f.TipProdaje}, Plaćanje: {f.NacinPlacanja}");
            }
        }
        private static KategorijaVina UnesiKategoriju()
        {
            Console.WriteLine("Izaberite kategoriju vina:");
            Console.WriteLine("1. Stolno vino");
            Console.WriteLine("2. Kvalitetno vino");
            Console.WriteLine("3. Premijum vino");
            Console.Write("Opcija: ");

            while (true)
            {
                string? s = Console.ReadLine();
                if (int.TryParse(s, out int op))
                {
                    return op switch
                    {
                        1 => KategorijaVina.STOLNO_VINO,
                        2 => KategorijaVina.KVALITETNO_VINO,
                        3 => KategorijaVina.PREMIJUM_VINO,
                        _ => Ponavljaj("Nepoznata kategorija. Unesi 1-3: ")
                    };
                }
                Console.Write("Neispravan unos. Unesi 1-3: ");
            }
        }

        private static TipProdaje UnesiTipProdaje()
        {
            Console.WriteLine("Izaberite tip prodaje:");
            Console.WriteLine("1. Restoranska prodaja");
            Console.WriteLine("2. Diskont pica");
            Console.Write("Opcija: ");

            while (true)
            {
                string? s = Console.ReadLine();
                if (int.TryParse(s, out int op))
                {
                    return op switch
                    {
                        1 => TipProdaje.RESTORANSKA_PRODAJA,
                        2 => TipProdaje.DISKONT_PICA,
                        _ => PonavljajTip("Nepoznat tip prodaje. Unesi 1-2: ")
                    };
                }
                Console.Write("Neispravan unos. Unesi 1-2: ");
            }
        }

        private static NacinPlacanja UnesiNacinPlacanja()
        {
            Console.WriteLine("Izaberite način placanja:");
            Console.WriteLine("1. Gotovina");
            Console.WriteLine("2. Predracun");
            Console.WriteLine("3. Gotovinski racun");
            Console.Write("Opcija: ");

            while (true)
            {
                string? s = Console.ReadLine();
                if (int.TryParse(s, out int op))
                {
                    return op switch
                    {
                        1 => NacinPlacanja.GOTOVINA,
                        2 => NacinPlacanja.PREDRACUN,
                        3 => NacinPlacanja.GOTOVINSKI_RACUN,
                        _ => PonavljajPlacanje("Nepoznat nacin. Unesi 1-3: ")
                    };
                }
                Console.Write("Neispravan unos. Unesi 1-3: ");
            }
        }

        private static KategorijaVina Ponavljaj(string msg)
        {
            Console.Write(msg);
            return UnesiKategoriju();
        }

        private static TipProdaje PonavljajTip(string msg)
        {
            Console.Write(msg);
            return UnesiTipProdaje();
        }

        private static NacinPlacanja PonavljajPlacanje(string msg)
        {
            Console.Write(msg);
            return UnesiNacinPlacanja();
        }
    }
}