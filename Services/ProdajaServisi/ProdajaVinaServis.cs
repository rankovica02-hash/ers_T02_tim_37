using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Servisi;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ProdajaServisi
{
    public class ProdajaVinaServis : IFakturaPregledServis
    {
        IFakturaRepozitorijum fakturaRepozitorijum;
        ILoggerServis loggerServis;
        public ProdajaVinaServis(IFakturaRepozitorijum fakturaRepozitorijum, ILoggerServis loggerServis)
        {
            this.fakturaRepozitorijum = fakturaRepozitorijum;
            this.loggerServis = loggerServis;

        }

        public IEnumerable<Faktura> PregledSvihFaktura()
        {
            try
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, "Pregled svih faktura (glavni enolog)");
                return fakturaRepozitorijum.SveFakture();
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Neuspesan pregled svih faktura");
                return [];
            }
        }
    }
}
