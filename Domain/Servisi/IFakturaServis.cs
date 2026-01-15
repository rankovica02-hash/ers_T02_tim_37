using Domain.Modeli;

namespace Domain.Servisi
{
    public interface IFakturaPregledServis
    {
        IEnumerable<Faktura> PregledSvihFaktura();
    }
}
