using Domain.Enumeracije;
using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Servisi
{
    public interface IProdajaVinaServis
    {

        IEnumerable<Vino> PrikaziKatalog();
        Faktura ProdajaIzKataloga(long vinoId, int brojFlasa, TipProdaje tipProdaje, NacinPlacanja nacinPlacanja);
        IEnumerable<Faktura> PregledSvihFaktura();
    }
}
