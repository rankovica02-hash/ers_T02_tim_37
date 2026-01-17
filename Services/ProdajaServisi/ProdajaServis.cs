using Domain.Enumeracije;
using Domain.Konstante;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;

namespace Services.ProdajaServisi
{
    public class ProdajaServis : IProdajaVinaServis
    {
        IVinaRepozitorijum vinaRepo;
        IKatalogVinaRepozitorijum katalogRepo;
        IFakturaRepozitorijum fakturaRepo;
        IPakovanjeServis pakovanjeServis;
        ILoggerServis logger;

        public ProdajaServis(
            IVinaRepozitorijum vinaRepo,
            IKatalogVinaRepozitorijum katalogRepo,
            IFakturaRepozitorijum fakturaRepo,
            IPakovanjeServis pakovanjeServis,
            ILoggerServis logger)
        {
            this.vinaRepo = vinaRepo;
            this.katalogRepo = katalogRepo;
            this.fakturaRepo = fakturaRepo;
            this.pakovanjeServis = pakovanjeServis;
            this.logger = logger;
        }

        public IEnumerable<Vino> PrikaziKatalog()
        {
            var katalog = katalogRepo.PronadjiSveKataloge().FirstOrDefault();
            if (katalog == null) return [];

            List<Vino> list = [];
            foreach (var id in katalog.VinaIds)
            {
                var v = vinaRepo.PronadjiVinoPoId(id);
                if (v != null) list.Add(v);
            }
            return list;
        }

        public IEnumerable<Faktura> PregledSvihFaktura()
        {
            return fakturaRepo.SveFakture();
        }

        public Faktura Prodaj(
            string nazivSorte,
            KategorijaVina kategorija,
            int brojFlasa,
            double zapreminaLitara,
            string adresaOdredista,
            long vinskiPodrumId,
            TipProdaje tipProdaje,
            NacinPlacanja nacinPlacanja)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nazivSorte) || brojFlasa <= 0 || zapreminaLitara <= 0 || string.IsNullOrWhiteSpace(adresaOdredista) || vinskiPodrumId <= 0)
                {
                    logger.EvidentirajDogadjaj(TipEvidencije.WARNING, "Prodaja - nevalidni ulazni podaci.");
                    return new Faktura();
                }

                
                int maxPoPaleti = BrojVinaPoPaleti.brojVinaPoPaleti;

                int preostalo = brojFlasa;
                while (preostalo > 0)
                {
                    int tura = Math.Min(maxPoPaleti, preostalo);


                    Paleta nova = pakovanjeServis.SpakujVinaUNovuPaletu(
                        kategorija,
                        tura,
                        zapreminaLitara,
                        nazivSorte,
                        adresaOdredista,
                        vinskiPodrumId
                    );

                    if (nova == null || nova.Id == 0)
                    {
                        logger.EvidentirajDogadjaj(TipEvidencije.ERROR, "Prodaja - neuspesno pakovanje.");
                        return new Faktura();
                    }


                    Paleta poslata = pakovanjeServis.PosaljiPaletuUVinskiPodrum(nova, vinskiPodrumId);
                    if (poslata == null || poslata.Id == 0)
                    {
                        logger.EvidentirajDogadjaj(TipEvidencije.ERROR, "Prodaja - neuspesno slanje palete.");
                        return new Faktura();
                    }

                    preostalo -= tura;
                }

                Faktura faktura = new Faktura
                {
                    TipProdaje = tipProdaje,
                    NacinPlacanja = nacinPlacanja,
                    DatumIzdavanja = DateTime.Now
                };

                faktura = fakturaRepo.DodajFakturu(faktura);

                logger.EvidentirajDogadjaj(TipEvidencije.INFO,
                    $"Prodaja - kreirana faktura Id={faktura.Id} sorta={nazivSorte} kom={brojFlasa} zap={zapreminaLitara}");

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
