using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Servisi;
namespace Presentation.Meni
{
    public class OpcijeMeni
    {
        private readonly IPaletaServis paletaServis;
        public OpcijeMeni(IPaletaServis paleta)
        {
            paletaServis = paleta;
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
    }
}
