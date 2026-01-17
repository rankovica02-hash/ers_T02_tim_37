using Domain.Enumeracije;
using Domain.Modeli;
namespace Domain.Servisi
{
    public interface IPakovanjeServis
    {
        Paleta SpakujVinaUNovuPaletu(KategorijaVina kategorija, int brojFlasa, double zapreminaLitara, string nazivSorte, string adresaOdredista, long vinskiPodrumId);
        Paleta PosaljiPaletuUVinskiPodrum(Paleta paleta, long vinskiPodrumId);
    }
}
