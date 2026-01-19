using Domain.Enumeracije;

namespace Domain.Modeli
{
    public class Faktura
    {
        public long Id { get; set; } = 0;
        public DateTime DatumIzdavanja { get; set; }
        public TipProdaje TipProdaje { get; set; }
        public NacinPlacanja NacinPlacanja { get; set; } 
        public List<StavkeFakture> Stavke { get; set; } = new();
        public decimal UkupanIznos => Stavke.Sum(s => s.UkupnaCena);

        public Faktura()
        {
            DatumIzdavanja = DateTime.Now;
        }

    }
}
