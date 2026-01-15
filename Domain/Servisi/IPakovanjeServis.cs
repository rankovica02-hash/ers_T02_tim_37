using Domain.Enumeracije;
using Domain.Modeli;
namespace Domain.Servisi
{
    public interface IPakovanjeServis
    {
        Paleta SpakujVinaUNovuPaletu(KategorijaVina kategorija, int brojFlasa, ZapreminaFlase zapremina, string nazivSorte, string adresaOdredista, long vinskiPodrumId);
        Paleta PosaljiPaletuUVinskiPodrum(KategorijaVina kategorija, int brojFlasa, ZapreminaFlase zapremina, string nazivSorte, string adresaOdredista, long vinskiPodrumId);
    }
}
