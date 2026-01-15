namespace Domain.Modeli
{
    public class StavkeFakture
    {
        public long VinoId { get; set; } 
        public string NazivVina { get; set; } = string.Empty;
        public int Kolicina { get; set; }
        public decimal CenaPoFlasi { get; set; }
        public decimal UkupnaCena => Kolicina * CenaPoFlasi;

    }
}
