using Domain.Enumeracije;
using Domain.Modeli;
namespace Domain.Servisi
{
    public interface ISkladistenjeServis
    {
        List<Paleta> IsporuciPaleteServisuProdaje(int brojPaleta);
    }
}
