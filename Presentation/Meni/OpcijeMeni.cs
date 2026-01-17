using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Servisi;
namespace Presentation.Meni
{
    public class OpcijeMeni
    {
        IProdajaVinaServis prodajaServis;
        public OpcijeMeni(IProdajaVinaServis prodajaServis)
        {
            this.prodajaServis = prodajaServis;
        }
        public void PrikaziMeni()
        {
            bool kraj = false;
            while (!kraj)
            {
                Console.WriteLine("\n============================================ Meni ===========================================");
                Console.WriteLine("\n1. Katalog vina");
                Console.WriteLine("2. Prodaja vina");
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
            var vina = prodajaServis.PrikaziKatalog().ToList();
            Console.WriteLine("\n=== KATALOG VINA ===");

            if (vina.Count == 0)
            {
                Console.WriteLine("Katalog je prazan.");
                return;
            }

            Console.WriteLine(Vino.Header());
            foreach (var v in vina)
                Console.WriteLine(v.ToString());
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

            KategorijaVina kategorija = UnesiKategoriju();

            Console.Write("Unesite broj flasa: ");
            if (!int.TryParse(Console.ReadLine(), out int brojFlasa) || brojFlasa <= 0)
            {
                Console.WriteLine("Broj flasa nije validan.");
                return;
            }

            Console.Write("Unesite zapreminu flase u litrima (0.75 ili 1.5): ");
            if (!double.TryParse(Console.ReadLine(), out double zapremina) || zapremina <= 0)
            {
                Console.WriteLine("Zapremina nije validna.");
                return;
            }

            TipProdaje tipProdaje = UnesiTipProdaje();
            NacinPlacanja nacinPlacanja = UnesiNacinPlacanja();

            Console.Write("Unesite adresu odredista: ");
            string adresa = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(adresa))
            {
                Console.WriteLine("Adresa ne sme biti prazna.");
                return;
            }

            Console.Write("Unesite ID vinskog podruma: ");
            if (!long.TryParse(Console.ReadLine(), out long podrumId) || podrumId <= 0)
            {
                Console.WriteLine("PodrumId nije validan.");
                return;
            }

            Faktura faktura = prodajaServis.Prodaj(
                nazivSorte.Trim(),
                kategorija,
                brojFlasa,
                zapremina,
                adresa.Trim(),
                podrumId,
                tipProdaje,
                nacinPlacanja
            );

            if (faktura == null || faktura.Id == 0)
            {
                Console.WriteLine("Prodaja nije uspela.");
                return;
            }

            Console.WriteLine($"Prodaja uspesna! Kreirana faktura (Id): {faktura.Id}");
        }

        private void PregledFakture()
        {
            var fakture = prodajaServis.PregledSvihFaktura().ToList();
            Console.WriteLine("\n=== FAKTURE ===");

            if (fakture.Count == 0)
            {
                Console.WriteLine("Nema faktura (još nije bilo prodaje).");
                return;
            }

            foreach (var f in fakture)
            {
                Console.WriteLine($"Id: {f.Id}, Datum: {f.DatumIzdavanja:dd.MM.yyyy HH:mm}, Tip: {f.TipProdaje}, Plaćanje: {f.NacinPlacanja}, Ukupno: {f.UkupanIznos}");
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