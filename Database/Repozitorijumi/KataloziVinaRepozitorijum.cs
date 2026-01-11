using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class KataloziVinaRepozitorijum : IKatalogVinaRepozitorijum
    {
        IBazaPodataka bazaPodataka;

        public KataloziVinaRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public KatalogVina DodajKatalogVina(KatalogVina katalog)
        {
            try
            {
                katalog.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                bazaPodataka.Tabele.KataloziVina.Add(katalog);
                bool sacuvano = bazaPodataka.SacuvajPromene();

                if (sacuvano)
                    return katalog;
                else
                    return new KatalogVina();
            }
            catch
            {
                return new KatalogVina();
            }

        }
        public bool AzurirajKatalogVina(KatalogVina katalog)
        {
            try
            {
                for (int i = 0; i < bazaPodataka.Tabele.KataloziVina.Count; i++)
                {
                    if (bazaPodataka.Tabele.KataloziVina[i].Id == katalog.Id)
                    {
                        bazaPodataka.Tabele.KataloziVina[i] = katalog;
                        return bazaPodataka.SacuvajPromene();
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

       public KatalogVina PronadjiKatalogPoId(long id)
        {
            try
            {
                foreach (KatalogVina katalog in bazaPodataka.Tabele.KataloziVina)
                {
                    if (katalog.Id == id)
                    {
                        return katalog;
                    }
                }
                return new KatalogVina();
            }
            catch
            {
                return new KatalogVina();
            }
        }


        public IEnumerable<KatalogVina> PronadjiSveKataloge()
        {
            try
            {
                return bazaPodataka.Tabele.KataloziVina;
            }
            catch
            {
                return new List<KatalogVina>();
            }
        }
    }
}
