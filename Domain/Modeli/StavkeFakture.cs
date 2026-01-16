namespace Domain.Modeli
{
    public class StavkeFakture
    {
        public long VinoId { get; set; } 
        public string NazivVina { get; set; } = string.Empty;
        public int Kolicina { get; set; }
        public decimal CenaPoFlasi { get; set; }
        public decimal UkupnaCena => Kolicina * CenaPoFlasi;
        public StavkeFakture() { }
        public StavkeFakture(long vinoId, string naziv, int kolicina, decimal cenaPoFlasi)
        {
            VinoId = vinoId;
            NazivVina = naziv;
            Kolicina = kolicina;
            CenaPoFlasi = cenaPoFlasi;
        }
    }
}
