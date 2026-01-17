using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Domain.PomocneMetode;
using Domain.PomocneMetode.Vino;
using Domain.Konstante;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Proizvodnja
{
    public class ProizvodnjeVinaServis : IProizvodnjaVinaServis
    {
        private readonly IVinogradarstvoServis lozaServis;
        private readonly IVinaRepozitorijum vinaRepozitorijum;
        private readonly ILoggerServis loggerServis;

        private const float optimalanSecer = 24.0f;

        public ProizvodnjeVinaServis(IVinogradarstvoServis lozaservis, IVinaRepozitorijum vinarepozitorijum, ILoggerServis loggerservis)
        {
            lozaServis = lozaservis;
            vinaRepozitorijum = vinarepozitorijum;
            loggerServis = loggerservis;
        }

        public IEnumerable<Vino> PocetakFermentacije(KategorijaVina kategorija, int brojFlasa, double zapreminaLitara, string nazivSorte)
        {
            try
            {
                List<Vino> proizvedenaVina = [];

                if (brojFlasa <= 0)
                {
                    loggerServis.EvidentirajDogadjaj(
                        TipEvidencije.WARNING,
                        "Broj flasa mora biti veci od nule.");
                    return [];
                }

                if (string.IsNullOrWhiteSpace(nazivSorte))
                {
                    loggerServis.EvidentirajDogadjaj(
                        TipEvidencije.WARNING,
                        "Naziv sorte ne sme biti prazan.");
                    return [];
                }

                double ukupnoLitara = brojFlasa * zapreminaLitara;
                int potrebanBrojLoza =
                    (int)Math.Ceiling(ukupnoLitara / ZapreminaFlase.LitaraPoLozi);

                loggerServis.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    $"Zapoceta fermentacija za {brojFlasa} flasa vina kategorije {kategorija}.");

                List<VinovaLoza> obraneLoze =
                    lozaServis.OberiLozeJedneSorte(nazivSorte, potrebanBrojLoza).ToList();

                while (obraneLoze.Count < potrebanBrojLoza)
                {
                    loggerServis.EvidentirajDogadjaj(
                        TipEvidencije.WARNING,
                        $"Nedovoljno loza sorte {nazivSorte}. Sadi se nova loza.");

                    VinovaLoza nova = lozaServis.PosadiNovuLozu(nazivSorte);
                    obraneLoze.Add(nova);
                }

                foreach (VinovaLoza obrana in obraneLoze)
                {
                    if (obrana.NivoSecera > optimalanSecer)
                    {
                        float visak = obrana.NivoSecera - optimalanSecer;

                        loggerServis.EvidentirajDogadjaj(
                            TipEvidencije.WARNING,
                            "Previsok nivo secera. Balansiranje sadnjom nove loze.");

                        VinovaLoza nova = lozaServis.PosadiNovuLozu(nazivSorte);

                        float procenat =
                            -((visak / nova.NivoSecera) * 100.0f);

                        lozaServis.PromeniNivoSeceraZaProcenat(nova.Id, procenat);
                    }
                }

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
                        zapreminaLitara,
                        "",
                        loza.Id,
                        datumFlasiranja
                    );

                    Vino dodato = vinaRepozitorijum.DodajVino(novoVino);

                    if (dodato.Id != 0)
                        proizvedenaVina.Add(dodato);

                    brojFlasa--;
                }

                loggerServis.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    $"Uspesno proizvedeno {proizvedenaVina.Count} vina kategorije {kategorija}.");

                return proizvedenaVina;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(
                    TipEvidencije.ERROR,
                    "Neuspesna fermentacija/proizvodnja vina.");
                return [];
            }
        }

        public IEnumerable<Vino> ZahtevajProizvedenaVina(
            KategorijaVina kategorija,
            int brojFlasa,
            double zapreminaLitara,
            string nazivSorte)
        {
            try
            {
                IEnumerable<Vino> proizvedena =
                    vinaRepozitorijum.PronadjiVinaPoKategoriji(kategorija);

                List<Vino> trazenaVina = [];

                foreach (Vino v in proizvedena)
                {
                    if (v.Zapremina == zapreminaLitara && brojFlasa > 0)
                    {
                        trazenaVina.Add(v);
                        brojFlasa--;
                    }
                }

                if (brojFlasa > 0)
                {
                    IEnumerable<Vino> novo =
                        PocetakFermentacije(
                            kategorija,
                            brojFlasa,
                            zapreminaLitara,
                            nazivSorte);

                    trazenaVina.AddRange(novo);
                }

                loggerServis.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    $"Dobijeno {trazenaVina.Count} vina kategorije {kategorija}.");

                return trazenaVina;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(
                    TipEvidencije.ERROR,
                    "Neuspesno dobijanje proizvedenih vina.");
                return [];
            }
        }
    }
}
