using Domain.Enumeracije;
using Domain.Konstante;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.ProdajaServisi
{
    public class ProdajaServis : IProdajaVinaServis
    {
        private readonly IVinaRepozitorijum vinaRepo;
        private readonly IKatalogVinaRepozitorijum katalogRepo;
        private readonly IFakturaRepozitorijum fakturaRepo;
        private readonly IPakovanjeServis pakovanjeServis;
        private readonly ISkladistenjeServis skladistenjeServis;
        private readonly ILoggerServis logger;

        public ProdajaServis(
            IVinaRepozitorijum vinaRepo,
            IKatalogVinaRepozitorijum katalogRepo,
            IFakturaRepozitorijum fakturaRepo,
            IPakovanjeServis pakovanjeServis,
            ISkladistenjeServis skladistenjeServis,
            ILoggerServis logger)
        {
            this.vinaRepo = vinaRepo;
            this.katalogRepo = katalogRepo;
            this.fakturaRepo = fakturaRepo;
            this.pakovanjeServis = pakovanjeServis;
            this.skladistenjeServis = skladistenjeServis;
            this.logger = logger;
        }
        private const long DefaultPodrumId = 1;

        public IEnumerable<Vino> PrikaziKatalog()
        {
            var katalog = katalogRepo.PronadjiSveKataloge().FirstOrDefault();
            if (katalog == null) return [];

            List<Vino> list = [];
            foreach (var id in katalog.VinaIds)
            {
                var v = vinaRepo.PronadjiVinoPoId(id);
                if (v != null && v.Id != 0) list.Add(v);
            }
            return list;
        }

        public IEnumerable<Faktura> PregledSvihFaktura()
        {
            return fakturaRepo.SveFakture();
        }

        public Faktura Prodaj(string nazivSorte,KategorijaVina kategorija,int brojFlasa,double zapreminaLitara,string adresaOdredista,TipProdaje tipProdaje,NacinPlacanja nacinPlacanja)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nazivSorte) || brojFlasa <= 0 || zapreminaLitara <= 0 || string.IsNullOrWhiteSpace(adresaOdredista))
                {
                    logger.EvidentirajDogadjaj(TipEvidencije.WARNING, "Prodaja - nevalidni ulazni podaci.");
                    return new Faktura();
                }

                int maxPoPaleti = BrojVinaPoPaleti.brojVinaPoPaleti;
                int brojPaletaZaIsporuku = (int)Math.Ceiling(brojFlasa / (double)maxPoPaleti);

                List<Paleta> upravoPoslate = new();

                int preostalo = brojFlasa;
                while (preostalo > 0)
                {
                    int tura = Math.Min(maxPoPaleti, preostalo);

                    Paleta nova = pakovanjeServis.SpakujVinaUNovuPaletu(kategorija,tura,zapreminaLitara,nazivSorte,adresaOdredista,DefaultPodrumId);

                    if (nova == null || nova.Id == 0)
                    {
                        logger.EvidentirajDogadjaj(TipEvidencije.ERROR, "Prodaja - neuspesno pakovanje.");
                        return new Faktura();
                    }

                    Paleta poslata = pakovanjeServis.PosaljiPaletuUVinskiPodrum(nova, DefaultPodrumId);
                    if (poslata == null || poslata.Id == 0)
                    {
                        logger.EvidentirajDogadjaj(TipEvidencije.ERROR, "Prodaja - neuspesno slanje palete.");
                        return new Faktura();
                    }

                    upravoPoslate.Add(poslata);
                    preostalo -= tura;
                }

                logger.EvidentirajDogadjaj(TipEvidencije.INFO,$"Prodaja - kreirano i poslato {upravoPoslate.Count} paleta za trazeno {brojPaletaZaIsporuku}.");
                List<Paleta> sveIsporucene = upravoPoslate;

                List<Vino> isporucenaVina = new();
                foreach (var paleta in sveIsporucene)
                {
                    if (paleta?.VinaIds == null) continue;

                    foreach (var vinoId in paleta.VinaIds)
                    {
                        var v = vinaRepo.PronadjiVinoPoId(vinoId);
                        if (v != null && v.Id != 0)
                            isporucenaVina.Add(v);
                    }
                }
                isporucenaVina = isporucenaVina.Take(brojFlasa).ToList();

                if (isporucenaVina.Count < brojFlasa)
                {
                    logger.EvidentirajDogadjaj(
                        TipEvidencije.ERROR,
                        $"Prodaja - nema dovoljno vina nakon isporuke. Trazeno={brojFlasa}, dobijeno={isporucenaVina.Count}."
                    );
                    return new Faktura();
                }

                Faktura faktura = new Faktura
                {
                    TipProdaje = tipProdaje,
                    NacinPlacanja = nacinPlacanja,
                    DatumIzdavanja = DateTime.Now
                };

                faktura.Stavke.Add(new StavkeFakture(
                    vinoId: 0,
                    naziv: $"{nazivSorte} ({kategorija}, {zapreminaLitara}L)",
                    kolicina: brojFlasa,
                    cenaPoFlasi: 0m
                ));

                faktura = fakturaRepo.DodajFakturu(faktura);

                logger.EvidentirajDogadjaj(TipEvidencije.INFO,$"Prodaja - kreirana faktura Id={faktura.Id} sorta={nazivSorte} kom={brojFlasa} zap={zapreminaLitara}");

                return faktura;
            }
            catch
            {
                logger.EvidentirajDogadjaj(TipEvidencije.ERROR, "Prodaja - izuzetak.");
                return new Faktura();
            }
        }
    }
}
