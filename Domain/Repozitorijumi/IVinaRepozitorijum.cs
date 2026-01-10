using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IVinaRepozitorijum
    {
        public Vino DodajVino(Vino vino);
        public Vino PronadjiVinoPoId(long id);
        public bool AzurirajVino(Vino vino);
        public IEnumerable<Vino> PronadjiVinoPoNazivu(string naziv);
    }
}
