using Domain.Modeli;

namespace Domain.BazaPodataka
{
    public class TabeleBazaPodataka
    {
        public List<Korisnik> Korisnici { get; set; } = [];
        // TODO: Add other database tables as needed
        public List<VinovaLoza> VinoveLoze { get; set; } = [];
        public List<Vino> Vina { get; set; } = [];

        public List<Paleta> Palete { get; set; } = [];
        public List<VinskiPodrum> VinskiPodrumi { get; set; } = [];
        public List<KatalogVina> KataloziVina { get; set; } = [];
        public TabeleBazaPodataka() { }
        public List<Faktura> Fakture { get; set; } = new();
    }
}
