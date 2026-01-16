using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IFakturaRepozitorijum
    {
        Faktura DodajFakturu(Faktura faktura);
        IEnumerable<Faktura> SveFakture();
    }
}
