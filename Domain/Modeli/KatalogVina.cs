
namespace Domain.Modeli
{
    public class KatalogVina
    {
        public long Id { get; set; } = 0;
        public string Naziv { get; set; } = string.Empty;
        public List<long> VinaIds { get; set; } = new List<long>();

        public KatalogVina() { }
        public KatalogVina(long id, string naziv, List<long> vinaIds)
        {
            Id = id;
            Naziv = naziv;
            VinaIds = vinaIds;
        }
    }
}
