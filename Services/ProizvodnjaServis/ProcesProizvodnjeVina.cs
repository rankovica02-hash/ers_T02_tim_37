using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Proizvodnja
{
    public class ProcesProizvodnjeVina : IProizvodnjaVinaServis
    {
        IVinogradarstvoServis lozaServis;
        IVinaRepozitorijum vinaRepozitorijum;
        ILoggerServis loggerServis;

        const int ClPoObranojLozi = 120;
        const float optimalanSecer = 24.0f;
        public ProcesProizvodnjeVina(IVinogradarstvoServis lozaservis, IVinaRepozitorijum vinarepozitorijum, ILoggerServis loggerservis)
        {
            lozaServis = lozaservis;
            vinaRepozitorijum = vinarepozitorijum;
            loggerServis = loggerservis;   
        }
        public IEnumerable<Vino> PocetakFermentacije(KategorijaVina kategorija, int brojFlasa, ZapreminaFlase zapremina, string nazivSorte)
        {
            try
            {
                List<Vino> proizvedenaVina = [];
                if (brojFlasa <= 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Broj flasa mora biti veci od nule.");
                    return [];
                }
                if (string.IsNullOrWhiteSpace(nazivSorte))
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Naziv sorte ne sme biti prazan.");
                    return [];
                }

                int ukupnoCl = brojFlasa * (int)zapremina;
                int potrebanBrojLoza = ukupnoCl / ClPoObranojLozi;

                if (ukupnoCl % ClPoObranojLozi != 0)
                {
                    potrebanBrojLoza++;
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Zapoceta fermentacija za {brojFlasa} flasa vina kategorije {kategorija}.");

                List<VinovaLoza> obraneLoze = lozaServis.OberiLozeJedneSorte(nazivSorte, potrebanBrojLoza).ToList();

                while (obraneLoze.Count < potrebanBrojLoza)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Nedovoljno loza sorte {nazivSorte}. Sadi se nova loza");
                    lozaServis.PosadiNovuLozu(nazivSorte);

                    foreach (VinovaLoza loza in lozaServis.OberiLozeJedneSorte(nazivSorte, 1))
                    {
                        obraneLoze.Add(loza);
                    }

                }

                foreach (VinovaLoza obrana in obraneLoze)
                {
                    if (obrana.NivoSecera > optimalanSecer)
                    {
                        float visak = obrana.NivoSecera - optimalanSecer;
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Previsok nivo secera u obranoj lozi. Balansiram secer sadnjom nove loze.");
                        VinovaLoza nova = lozaServis.PosadiNovuLozu(nazivSorte);
                        float cilj = nova.NivoSecera - visak;

                        float procenat = ((cilj - nova.NivoSecera) / nova.NivoSecera) * 100.0f;
                        lozaServis.PromeniNivoSeceraZaProcenat(nova.Id, procenat);
                    }

                }


            }
            catch { }
            
            throw new NotImplementedException();
        }


    }
}
