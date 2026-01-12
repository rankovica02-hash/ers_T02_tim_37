using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.Servisi
{
    public interface IVinogradarstvoServis
    {
        VinovaLoza PosadiNovuLozu(string nazivSorte);
        bool PromeniNivoSeceraZaProcenat(long vinovaLozaId, float procenat);
        IEnumerable<VinovaLoza> OberiLozeJedneSorte(string nazivSorte, int kolicina);
    }
}
