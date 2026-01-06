using Domain.Modeli;
using Domain.Enumeracije;

namespace Domain.Repozitorijumi
{
    public interface IPaletaRepozitorijum
    {
        public Paleta DodajPaletu(Paleta paleta);
        public Paleta PronadjiPaletuPoId(long id);
        public IEnumerable<Paleta> PronadjiSvePalete();
        public IEnumerable<Paleta> PronadjiPaletePoStatusu(TipStatusaPalete status);
        public bool AzurirajPaletu(Paleta paleta);
    }
}
