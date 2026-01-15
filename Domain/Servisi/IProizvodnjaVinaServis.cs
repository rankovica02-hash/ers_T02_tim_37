using Domain.Enumeracije;
using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Servisi
{
    public interface IProizvodnjaVinaServis
    {
       IEnumerable<Vino> PocetakFermentacije(KategorijaVina kategorija, int brojFlasa, ZapreminaFlase zapremina, string nazivSorte);
        IEnumerable<Vino> ZahtevajProizvedenaVina(KategorijaVina kategorija, int brojFlasa, ZapreminaFlase zapremina, string nazivSorte);
    }
}
