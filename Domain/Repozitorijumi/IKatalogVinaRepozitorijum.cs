using Domain.Modeli;
using Domain.Enumeracije;
namespace Domain.Repozitorijumi
{
    public interface IKatalogVinaRepozitorijum
    {
        public KatalogVina DodajKatalogVina(KatalogVina katalog);
        public KatalogVina PronadjiKatalogPoId(long id);
        public IEnumerable<KatalogVina> PronadjiSveKataloge();
        public bool AzurirajKatalogVina(KatalogVina katalog);
    }
}
