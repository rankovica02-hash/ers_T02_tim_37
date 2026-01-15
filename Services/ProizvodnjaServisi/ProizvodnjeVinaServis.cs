using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Domain.PomocneMetode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.PomocneMetode.Vino;

namespace Services.Proizvodnja
{
    public class ProizvodnjeVinaServis : IProizvodnjaVinaServis
    {
        IVinogradarstvoServis lozaServis;
        IVinaRepozitorijum vinaRepozitorijum;
        ILoggerServis loggerServis;

        const int ClPoObranojLozi = 120;
        const float optimalanSecer = 24.0f;
        public ProizvodnjeVinaServis(IVinogradarstvoServis lozaservis, IVinaRepozitorijum vinarepozitorijum, ILoggerServis loggerservis)
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

                        obraneLoze.Add(nova);
                    }

                }

                double zapreminaULitrima = ((int)zapremina / 100.0);
                DateTime datumFlasiranja = DateTime.Now;


                int index = 0;
                while (brojFlasa > 0)
                {
                    VinovaLoza loza = obraneLoze[index];
                    index = (index + 1) % obraneLoze.Count;
                    string naziv = NasumicanNazivVinaHelper.GenerisiNasumicanNazivVina();

                    Vino novoVino = new Vino(
                        0,
                        naziv,
                        kategorija,
                        zapreminaULitrima,
                        "",
                        loza.Id,
                        datumFlasiranja
                        );
                    Vino dodato = vinaRepozitorijum.DodajVino(novoVino);
                    if (dodato.Id != 0)
                    {
                        proizvedenaVina.Add(dodato);
                    }
                    else
                    {
                        continue;
                    }
                    brojFlasa--;
                }
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Uspesno proizvedeno {proizvedenaVina.Count()} vina kategorije {kategorija}.");
                return proizvedenaVina;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Neuspesna fermentacija/proizvodnja vina.");
                return [];
            }

        }

        public IEnumerable<Vino> ZahtevajProizvedenaVina(KategorijaVina kategorija, int brojFlasa, ZapreminaFlase zapremina, string nazivSorte)
        {
            try
            {
                IEnumerable<Vino> proizvedena = vinaRepozitorijum.PronadjiVinaPoKategoriji(kategorija);
                List<Vino> trazenaVina = [];
                double zapreminaULitrima = ((int)zapremina) / 100.0;

                foreach (Vino v in proizvedena)
                {
                    if (v.Zapremina == zapreminaULitrima && brojFlasa > 0)
                    {
                        trazenaVina.Add(v);
                        brojFlasa--;
                    }
                }
                if (brojFlasa > 0)
                {
                    IEnumerable<Vino> novoProizvedena = PocetakFermentacije(kategorija, brojFlasa, zapremina, nazivSorte);
                    trazenaVina.AddRange(novoProizvedena);
                }
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Proizvedeno je {trazenaVina.Count} vina kategorije {kategorija}");
                return trazenaVina;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Neuspesno dobijanje proizvedenih vina.");
                return [];
            }

        }
    }
}
