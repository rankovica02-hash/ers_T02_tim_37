using Domain.Enumeracije;

namespace Domain.Modeli
{
    public class Vino
    {
        public long Id { get; set; } = 0;
        public string Naziv { get; set; } = string.Empty;
        public KategorijaVina Kategorija { get; set; }
        public double Zapremina { get; set; } = 0;
        public string Sifra { get; set; } = string.Empty;   //VN-2025-ID_VINA
        public long VinovaLozaId { get; set; } = 0;
        public DateTime DatumFlasiranja { get; set; } = DateTime.MinValue;

        public Vino() { }

        public Vino(long id, string naziv, KategorijaVina kategorija, double zapremina, string sifra, long vinovalozaid, DateTime datumflasiranja)
        {
            Id = id;
            Naziv = naziv;
            Kategorija = kategorija;
            Zapremina = zapremina;
            Sifra = sifra;
            VinovaLozaId = vinovalozaid;
            DatumFlasiranja = datumflasiranja;
        }

        public static string Header()
        {
            return $@"| {"Naziv",-22} | {"Kategorija",-12} | {"Zapremina(L)",-12} | {"SifraSerije",-22} | {"VinovaLozaId",-15} | {"DatFlaširanja",-15} |" +
                   "\n---------------------------------------------------------------------------------------------------------------";
        }

        public override string ToString()
        {
            return $@"| {Naziv,-22} | {Kategorija,-12} | {Zapremina,-12:0.##} | {Sifra,-22} | {VinovaLozaId,-15} | {DatumFlasiranja:dd.MM.yyyy,-15} |";
        }
    }
}
