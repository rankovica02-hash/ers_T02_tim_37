using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.Servisi
{
    public interface IPaletaServis
    {
        public Paleta KreiranjePalete(string adresaOdredista, long vinskiPodrumId);
        public IEnumerable<Paleta> PrikazSvihPaleta();
        public IEnumerable<Paleta> PrikazPaletaPoStatusu(TipStatusaPalete status);
    }
}
