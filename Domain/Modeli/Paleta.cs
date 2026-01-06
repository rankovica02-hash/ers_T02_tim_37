using Domain.Enumeracije;


namespace Domain.Modeli
{
    public class Paleta
    {
        public long Id { get; set; } = 0;
        public string Sifra { get; set; }  = string.Empty;
        public string AdresaOdredista {  get; set; } = string.Empty;
        public long VinskiPodrumId { get; set; } = 0;
        public List<long> VinaIds { get; set; } = new List<long>();
        public TipStatusaPalete Status { get; set; }

        public Paleta() { }

        public Paleta(long id, string sifra, string adresaOdredista, long vinskiPodrumId, List<long>? vinaIds, TipStatusaPalete status)
        {
            Id = id;
            Sifra = sifra;
            AdresaOdredista = adresaOdredista;
            VinskiPodrumId = vinskiPodrumId;
            VinaIds = vinaIds ?? new List<long>();
            Status = status;
        }
    }
}
