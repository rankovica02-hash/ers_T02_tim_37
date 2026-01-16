using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Servisi;
namespace Presentation.Meni
{
    public class OpcijeMeni
    {
        IPaletaServis paletaServis;
        ISkladistenjeServis skladistenjeServis;
        public OpcijeMeni(IPaletaServis paleta, ISkladistenjeServis skladistenjeServ)
        {
            paletaServis = paleta;
            skladistenjeServis = skladistenjeServ;
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

       public void ZahtevIsporukeServisuProdaje()
        {
            Console.WriteLine("Unesite broj paleta za isporuku:");
            string? unos = Console.ReadLine();
            if(!int.TryParse(unos, out int brojPaleta))
            {
                Console.WriteLine("Unos nije validan. Molim Vas unesite ceo broj.");
                return;
            }
            var palete = skladistenjeServis.IsporuciPaleteServisuProdaje(brojPaleta);
            Console.WriteLine("Uspesna isporuka!");
        }

        /*private void IsporukaPaleta() isto kao ova andjina samo je moja, pa je bolja
        {
            Console.Write("Unesi broj paleta za isporuku servisu prodaje: ");
            if (!int.TryParse(Console.ReadLine(), out int broj) || broj <= 0)
            {
                Console.WriteLine("Neispravan broj.");
                return;
            }

            var isporucene = skladistenjeServis.IsporuciPaleteServisuProdaje(broj).ToList();

            Console.WriteLine($"Isporuceno paleta: {isporucene.Count}");
            foreach (var p in isporucene)
                Console.WriteLine($"- {p.Sifra} (Id={p.Id}, Status={p.Status})");
        }
        */
    }
}
