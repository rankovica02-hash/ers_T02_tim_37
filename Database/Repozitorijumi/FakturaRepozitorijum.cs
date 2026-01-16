using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class FakturaRepozitorijum : IFakturaRepozitorijum
    {
        private readonly IBazaPodataka baza;

        public FakturaRepozitorijum(IBazaPodataka bazaPodataka)
        {
            baza = bazaPodataka;
        }

        public Faktura DodajFakturu(Faktura faktura)
        {
            try
            {
                if (faktura.Id == 0)
                    faktura.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                baza.Tabele.Fakture.Add(faktura);
                bool sacuvano = baza.SacuvajPromene();

                return sacuvano ? faktura : new Faktura();
            }
            catch
            {
                return new Faktura();
            }
        }

        public IEnumerable<Faktura> SveFakture()
        {
            try
            {
                return baza.Tabele.Fakture;
            }
            catch
            {
                return [];
            }
        }
    }
}
