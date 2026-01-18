using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Services.Proizvodnja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.PakovanjeServisi
{
    public class PakovanjeServis : IPakovanjeServis
    {
        private readonly IPaletaRepozitorijum paletaRepozitorijum;
        private readonly ILoggerServis loggerServis;
        private readonly IProizvodnjaVinaServis proizvodnjaServis;

        public PakovanjeServis(IPaletaRepozitorijum paletaRepo, ILoggerServis logger, IProizvodnjaVinaServis proizvodnjaServ)
        {
            paletaRepozitorijum = paletaRepo;
            loggerServis = logger;
            proizvodnjaServis = proizvodnjaServ;
        }

        public Paleta SpakujVinaUNovuPaletu(KategorijaVina kategorija,int brojFlasa,double zapreminaFlase,string nazivSorte,string adresaOdredista,long vinskiPodrumId)
        {
            try
            {
                if (brojFlasa <= 0 || string.IsNullOrWhiteSpace(nazivSorte) || string.IsNullOrWhiteSpace(adresaOdredista) || vinskiPodrumId <= 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Pakovanje - ulazni podaci nisu validni.");
                    return new Paleta();
                }

                HashSet<long> vecSpakovanaVina = paletaRepozitorijum.PronadjiSvePalete().Where(p => p != null && p.VinaIds != null).SelectMany(p => p.VinaIds).ToHashSet();

                List<Vino> kandidati = proizvodnjaServis.ZahtevajProizvedenaVina(kategorija, brojFlasa, zapreminaFlase, nazivSorte).Where(v => v != null && v.Id != 0).GroupBy(v => v.Id).Select(g => g.First()).ToList();

                kandidati = kandidati.Where(v => !vecSpakovanaVina.Contains(v.Id)).ToList();

                int fali = brojFlasa - kandidati.Count;
                if (fali > 0)
                {
                    loggerServis.EvidentirajDogadjaj(
                        TipEvidencije.INFO,
                        $"Pakovanje - nema dovoljno slobodnih vina, fali {fali}. Pokrece se proizvodnja.");

                    List<Vino> dodatna = proizvodnjaServis.PocetakFermentacije(kategorija, fali, zapreminaFlase, nazivSorte).Where(v => v != null && v.Id != 0).ToList();

                    kandidati.AddRange(dodatna);
                }

                if (kandidati.Count < brojFlasa)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR,$"Pakovanje - nema dovoljno SLOBODNIH vina. Trazeno={brojFlasa}, dobijeno={kandidati.Count}.");
                    return new Paleta();
                }

                List<Vino> listaVina = kandidati.Take(brojFlasa).ToList();

                Paleta novaPaleta = new Paleta
                {
                    AdresaOdredista = adresaOdredista,
                    VinskiPodrumId = vinskiPodrumId,
                    VinaIds = listaVina.Select(v => v.Id).ToList(),
                    Status = TipStatusaPalete.UPAKOVANA
                };

                novaPaleta = paletaRepozitorijum.DodajPaletu(novaPaleta);
                if (novaPaleta == null || novaPaleta.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Pakovanje - neuspesno cuvanje palete.");
                    return new Paleta();
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Pakovanje - kreirana nova paleta {novaPaleta.Sifra}.");
                return novaPaleta;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Pakovanje - izuzetak pri pakovanju vina u paletu.");
                return new Paleta();
            }
        }

        public Paleta PosaljiPaletuUVinskiPodrum(Paleta paleta, long vinskiPodrumId)
        {
            try
            {
                if (paleta == null || paleta.Id == 0 || vinskiPodrumId <= 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Slanje palete - paleta/podrum nisu validni.");
                    return new Paleta();
                }

                if (paleta.Status == TipStatusaPalete.OTPREMLJENA)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Slanje palete - paleta je vec poslata (OTPREMLJENA).");
                    return new Paleta();
                }

                paleta.VinskiPodrumId = vinskiPodrumId;
                paleta.Status = TipStatusaPalete.OTPREMLJENA;

                bool uspesno = paletaRepozitorijum.AzurirajPaletu(paleta);
                if (!uspesno)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Slanje palete - neuspesno azuriranje palete.");
                    return new Paleta();
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Slanje palete - paleta {paleta.Sifra} uspesno poslata u podrum.");
                return paleta;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Slanje palete - izuzetak prilikom slanja palete u podrum.");
                return new Paleta();
            }
        }
    }
}
