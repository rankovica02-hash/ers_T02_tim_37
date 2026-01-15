using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Services.Proizvodnja;


namespace Services.PakovanjeServisi
{
    public class PakovanjeServis : IPakovanjeServis
    {
        IPaletaRepozitorijum paletaRepozitorijum;
        ILoggerServis loggerServis;
        IProizvodnjaVinaServis proizvodnjaServis;
        // treba jos proizvodnja vina servis 

        //i ovde da se doda za proizvodnju
        public PakovanjeServis(IPaletaRepozitorijum paletaRepo, ILoggerServis logger, IProizvodnjaVinaServis proizvodnjaServ)
        {
            paletaRepozitorijum = paletaRepo;
            loggerServis = logger;
            proizvodnjaServis = proizvodnjaServ;
        }

        public Paleta SpakujVinaUNovuPaletu(KategorijaVina kategorija, int brojFlasa, ZapreminaFlase zapremina, string nazivSorte, string adresaOdredista, long vinskiPodrumId)
        {
            try
            {
                if (brojFlasa <= 0 || string.IsNullOrWhiteSpace(nazivSorte) || string.IsNullOrWhiteSpace(adresaOdredista) || vinskiPodrumId <= 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Ulazni podaci nisu validni");
                    return new Paleta();
                }

                IEnumerable<Vino> vina = proizvodnjaServis.ZahtevajProizvedenaVina(kategorija, brojFlasa, zapremina, nazivSorte);
                List<Vino> listaVina = vina.ToList();


               if(listaVina.Count != brojFlasa)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Proizvodnja nije vratila trazenu kolicinu vina.");
                    return new Paleta();
                }

                HashSet<long> vecSpakovanaVina = paletaRepozitorijum.PronadjiSvePalete().SelectMany(p => p.VinaIds).ToHashSet();

                foreach (Vino v in listaVina)
                {
                    if (vecSpakovanaVina.Contains(v.Id))
                    {
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Vino je vec spakovano u paletu.");
                        return new Paleta();
                    }
                }

                // kreiranje nove palete
                Paleta novaPaleta = new Paleta
                {
                    AdresaOdredista = adresaOdredista,
                    VinskiPodrumId = vinskiPodrumId,
                    VinaIds = listaVina.Select(v => v.Id).ToList(),
                    Status = TipStatusaPalete.UPAKOVANA
                };
                
                novaPaleta =  paletaRepozitorijum.DodajPaletu(novaPaleta);
                if (novaPaleta.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Neuspesno cuvanje palete.");
                    return new Paleta();
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Kreirana je nova paleta {novaPaleta.Sifra}.");
                return novaPaleta;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Izuzetak pri pakovanju vina u palete.");
                return new Paleta();
            }
        }

        // za slanje palete u podrum
        public Paleta PosaljiPaletuUVinskiPodrum(KategorijaVina kategorija, int brojFlasa, ZapreminaFlase zapremina, string nazivSorte, string adresaOdredista, long vinskiPodrumId)
        {
            try
            {
                // prva dostupna paleta koja je upakovana
                Paleta paleta = paletaRepozitorijum.PronadjiPaletePoStatusu(TipStatusaPalete.UPAKOVANA).FirstOrDefault();
                if (paleta == null || paleta.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, "Trenutno nema upakovanih paleta, zapocinjanje novog pakovanja.");

                    paleta = SpakujVinaUNovuPaletu(kategorija,brojFlasa,zapremina, nazivSorte, adresaOdredista, vinskiPodrumId);

                    if(paleta.Id == 0)
                    return new Paleta();
                }

                // jedna paleta u jedan podrum
                if(paleta.Status == TipStatusaPalete.OTPREMLJENA)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Paleta je vec poslata!");
                    return new Paleta();
                }

                paleta.VinskiPodrumId = vinskiPodrumId;
                paleta.Status = TipStatusaPalete.OTPREMLJENA;

                bool uspesno = paletaRepozitorijum.AzurirajPaletu(paleta);
                if (!uspesno)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Neuspesno azuriranje palete.");
                    return new Paleta();
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Paleta {paleta.Sifra} uspesno poslata u podrum.");
                return paleta;

            }

            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Izuzetak prilikom slanja palete u podrum.");
                return new Paleta();
            }
        }
    }
}
