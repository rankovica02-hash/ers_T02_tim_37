using Domain.Enumeracije;

namespace Domain.Modeli
{
    public class VinovaLoza
    {
        public long Id { get; set; } = 0;
        public string Naziv { get; set; } = string.Empty;
        public float NivoSecera { get; set; } = 0;
        public int GodinaProizvodnje { get; set; } = 0;
        public string RegionUzgoja { get; set; } = string.Empty;
        public FazaZrelosti Faza { get; set; }

        public VinovaLoza() { }

        public VinovaLoza(long id, string naziv, float nivosecera, int godinaproizvodnje, string regionuzgoja, FazaZrelosti faza)
        {
            Id = id;
            Naziv = naziv;
            NivoSecera = nivosecera;
            GodinaProizvodnje = godinaproizvodnje;
            RegionUzgoja = regionuzgoja;
            Faza = faza;
        }
    }
}
